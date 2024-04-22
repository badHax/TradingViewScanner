using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TVScanner.Shared;
using TVScanner.Shared.Configuration;
using TVScanner.Shared.Logging;
using TVScanner.Shared.Notifications;
using TVScanner.Shared.Scanner;
namespace TVScanner.Jobs.Tests
{
    [TestFixture]
    public class ScannerTests
    {
        private TestScanner _testClass;
        private IServiceScopeFactory _scopeFactory;
        private ScanFilter _scanFilter;
        private string _messageType;

        private const int _interval = 200;
        private const int _purgeAfterDays = 7;
        private const string _scanUrl = "https://scanUrl.com/";
        private const string _purgeSchedule = "0 0 0 * * *";

        // services
        private IAbstractLogger _logger;
        private ICache _cache;
        private IScanService _scanService;
        private INotificationService _notificationService;
        private IOptions<AppConfig> _appConfig;
        private IRepositoryContext _repositoryContext;
        private ITaskDelayer _taskDelayer;
        private IScanRecordManager _scanRecordManager;

        [SetUp]
        public void SetUp()
        {
            _logger = Substitute.For<IAbstractLogger>();
            _cache = Substitute.For<ICache>();
            _scanService = Substitute.For<IScanService>();
            _notificationService = Substitute.For<INotificationService>();
            _appConfig = Substitute.For<IOptions<AppConfig>>();
            _appConfig.Value.Returns(new AppConfig() { ScannerConfig = new ScannerConfig() { Interval = _interval, PurgeAfterDays = _purgeAfterDays, Url = _scanUrl, PurgeSchedule = _purgeSchedule } });
            _repositoryContext = Substitute.For<IRepositoryContext>();
            _taskDelayer = Substitute.For<ITaskDelayer>();
            _scanRecordManager = Substitute.For<IScanRecordManager>();

            var services = new ServiceCollection();
            services.AddScoped((provider) => _logger);
            services.AddScoped((provider) => _cache);
            services.AddScoped((provider) => _scanService);
            services.AddScoped((provider) => _notificationService);
            services.AddScoped((provider) => _appConfig);
            services.AddScoped((provider) => _repositoryContext);
            services.AddScoped((provider) => _taskDelayer);
            services.AddScoped((provider) => _scanRecordManager);

            var serviceProvider = services.BuildServiceProvider();

            var scope = Substitute.For<IServiceScope>();
            scope.ServiceProvider.Returns(serviceProvider);

            _scopeFactory = Substitute.For<IServiceScopeFactory>();
            _scopeFactory.CreateScope().Returns(scope);

            _scanFilter = new ScanFilter(ScanType.HighOfDay);
            _messageType = Constants.MessageListeners.HighOfDay;
            _testClass = new TestScanner(_scopeFactory, _scanFilter, _messageType);
        }

        [Test]
        public void CannotConstructWithNullScopeFactory()
        {
            Should.Throw<ArgumentNullException>(() => new TestScanner(default(IServiceScopeFactory), _scanFilter, _messageType));
        }

        [Test]
        public void CannotConstructWithNullScanFilter()
        {
            Should.Throw<ArgumentNullException>(() => new TestScanner(_scopeFactory, default(ScanFilter), _messageType));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidMessageType(string value)
        {
            Should.Throw<ArgumentNullException>(() => new TestScanner(_scopeFactory, _scanFilter, value));
        }

        [Test]
        public async Task ExecuteAsync_ShouldDelayUntilCancelled()
        {
            // Arrange
            int count = 0;
            int expectedCount = 2;
            var cts = new CancellationTokenSource();
            var stoppingToken = cts.Token;

            _scanService.GetFilteredResult(_scanFilter)
                .Returns(Task.FromResult<ScanRecord[]>([]))
                .AndDoes(x => { if (++count >= expectedCount) cts.Cancel(); });

            // Act
            await _testClass.PublicExecuteAsync(stoppingToken);

            // Assert
            await _taskDelayer.Received(expectedCount).Delay(_interval, stoppingToken);
        }

        [Test]
        public async Task ExecuteAsync_NewRecords_ShouldSendNotificationsAndUpdateCache()
        {
            // Arrange
            var symbol = "XYZ";
            var existingRecords = new List<ScanRecord>();
            var newRecords = new List<ScanRecord> { new ScanRecord { Name = symbol } };

            var cts = new CancellationTokenSource();
            var stoppingToken = cts.Token;

            _cache.Get<IEnumerable<ScanRecord>>(_messageType).Returns(existingRecords);
            _scanService.GetFilteredResult(_scanFilter)
                .Returns(Task.FromResult(newRecords.ToArray()))
                .AndDoes(x => cts.Cancel()); // Manually stop job;
            _notificationService.SendPushNotification(Arg.Any<string>(), Arg.Any<ScanType>()).Returns(Task.CompletedTask);

            // Act
            await _testClass.PublicExecuteAsync(stoppingToken);

            // Assert
            await _notificationService.Received(1).SendPushNotification(symbol, _scanFilter.Type);
            await _cache.Received(1).Set(_messageType, Arg.Is<IEnumerable<ScanRecord>>(r => r.EqualBySymbolNames(newRecords)));
        }

        [Test]
        public async Task ExecuteAsync_SameRecordsAsCache_ShouldNotSendNotifications()
        {
            // Arrange
            var symbol = "ABC";
            var existingRecords = new List<ScanRecord> { new ScanRecord { Name = symbol } };
            var cts = new CancellationTokenSource();
            var stoppingToken = cts.Token;

            _cache.Get<IEnumerable<ScanRecord>>(_messageType).Returns(existingRecords);
            _scanService.GetFilteredResult(_scanFilter)
                .Returns(Task.FromResult(existingRecords.ToArray()))
                .AndDoes(x => cts.Cancel()); // Manually stop job;

            // Act
            await _testClass.PublicExecuteAsync(stoppingToken);

            // Assert
            await _notificationService.DidNotReceive().SendPushNotification(Arg.Any<string>(), Arg.Any<ScanType>());
        }

        [Test]
        public async Task ExecuteAsync_WhenExceptionOccurs_ShouldLogErrorAndContinue()
        {
            // Arrange
            var count = 0;
            var expectedCount = 2;
            var cts = new CancellationTokenSource();
            var stoppingToken = cts.Token;
            var expectedMessage = "Error: Test Exception";

            _scanService.GetFilteredResult(_scanFilter)
                .Throws(new Exception("Test Exception"));

            _logger.WhenForAnyArgs(x => x.LogError(Arg.Any<Scanner>(), default))
                .Do(x => { if (++count >= expectedCount) cts.Cancel(); });

            // Act
            await _testClass.PublicExecuteAsync(stoppingToken);

            // Assert
            await _scanService.Received(expectedCount).GetFilteredResult(_scanFilter);
            _logger.Received(expectedCount).LogError<Scanner>(Arg.Any<Scanner>(), expectedMessage);
        }

    }
}