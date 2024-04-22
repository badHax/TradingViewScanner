using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NCrontab;
using TVScanner.Shared.Configuration;
using TVScanner.Shared.Logging;
using TVScanner.Shared.Scanner;

namespace TVScanner.Jobs
{
    public class PurgeOldRecords : BackgroundService
    {
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly string chronExpression;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ITaskDelayer _taskDelayer;
        private readonly IAbstractLogger _logger;

        public PurgeOldRecords(
            IOptions<AppConfig> config,
            IServiceScopeFactory scopeFactory)
        {
            var serviceProvider = scopeFactory.CreateScope().ServiceProvider;
            chronExpression = config.Value.ScannerConfig.PurgeSchedule;
            _schedule = CrontabSchedule.Parse(chronExpression);
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            _scopeFactory = scopeFactory;
            _taskDelayer = serviceProvider.GetRequiredService<ITaskDelayer>();
            _logger = serviceProvider.GetRequiredService<IAbstractLogger>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                _logger.LogInformation(this, $"PurgeOldRecords is running at {now}");
                if (now > _nextRun)
                {
                    await Process();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }
                else
                {
                    _logger.LogInformation(this, $"PurgeOldRecords will run next at {_nextRun}");
                }

                var timespan = _nextRun - now;
                await _taskDelayer.Delay(timespan, stoppingToken);
            }
            while (!stoppingToken.IsCancellationRequested);
        }

        private async Task Process()
        {
            using var scope = _scopeFactory.CreateScope();
            var _scanManager = scope.ServiceProvider.GetRequiredService<IScanRecordManager>();
            await _scanManager.PurgeOldRecords();
        }
    }
}
