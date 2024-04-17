//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using NSubstitute;
//using NUnit.Framework;
//using Shouldly;
//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using TVScanner.Shared;
//using TVScanner.Shared.Configuration;
//using TVScanner.Shared.Notifications;
//using TVScanner.Shared.Scanner;

//namespace TVScanner.Jobs.Tests
//{
//    [TestFixture]
//    public class ScannerTests
//    {
//        private TestScanner _testClass;
//        private IServiceScopeFactory _scopeFactory;
//        private ScanFilter _scanFilter;
//        private string _messageType;
//        // services
//        private ILogger<Scanner> _logger;
//        private ICache _cache;
//        private ScanService _scanService;
//        private NotificationService _notificationService;
//        private IOptions<AppConfig> _appConfig;
//        private IRepositoryContext _repositoryContext;

//        [SetUp]
//        public void SetUp()
//        {
//            _logger = Substitute.For<ILogger<Scanner>>();
//            _cache = Substitute.For<ICache>();
//            _scanService = Substitute.For<ScanService>();
//            _notificationService = Substitute.For<NotificationService>();
//            _appConfig = Substitute.For<IOptions<AppConfig>>();
//            _repositoryContext = Substitute.For<IRepositoryContext>();

//            var serviceProvider = Substitute.For<IServiceProvider>();
//            serviceProvider.GetRequiredService<ILogger<Scanner>>().Returns(_logger);
//            serviceProvider.GetRequiredService<ICache>().Returns(_cache);
//            serviceProvider.GetRequiredService<ScanService>().Returns(_scanService);
//            serviceProvider.GetRequiredService<NotificationService>().Returns(_notificationService);
//            serviceProvider.GetRequiredService<IOptions<AppConfig>>().Returns(_appConfig);
//            serviceProvider.GetRequiredService<IRepositoryContext>().Returns(_repositoryContext);

//            var scope = Substitute.For<IServiceScope>();
//            scope.ServiceProvider.Returns(serviceProvider);

//            _scopeFactory = Substitute.For<IServiceScopeFactory>();
//            _scopeFactory.CreateScope().Returns(scope);

//            _scanFilter = new ScanFilter(ScanType.HighOfDay);
//            _messageType = "TestValue1470840479";
//            _testClass = new TestScanner(_scopeFactory, _scanFilter, _messageType);
//        }

//        [Test]
//        public void CannotConstructWithNullScopeFactory()
//        {
//            Should.Throw<ArgumentNullException>(() => new TestScanner(default(IServiceScopeFactory), _scanFilter, _messageType));
//        }

//        [Test]
//        public void CannotConstructWithNullScanFilter()
//        {
//            Should.Throw<ArgumentNullException>(() => new TestScanner(_scopeFactory, default(ScanFilter), _messageType));
//        }

//        [TestCase(null)]
//        [TestCase("")]
//        [TestCase("   ")]
//        public void CannotConstructWithInvalidMessageType(string value)
//        {
//            Should.Throw<ArgumentNullException>(() => new TestScanner(_scopeFactory, _scanFilter, value));
//        }
//    }
//}