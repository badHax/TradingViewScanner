using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NCrontab;
using TVScanner.Shared.Configuration;
using TVScanner.Shared.Scanner;

namespace TVScanner.Jobs
{
    public class PurgeOldRecords : BackgroundService
    {
        private CrontabSchedule _schedule;
        private DateTime _nextRun;
        private readonly string chronExpression;
        private readonly IServiceScopeFactory _scopeFactory;

        public PurgeOldRecords(
            IOptions<AppConfig> config,
            IServiceScopeFactory scopeFactory)
        {
            chronExpression = config.Value.ScannerConfig.PurgeSchedule;
            _schedule = CrontabSchedule.Parse(chronExpression);
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            do
            {
                var now = DateTime.Now;
                if (now > _nextRun)
                {
                    await Process();
                    _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
                }
                await Task.Delay(60 * 60 * 1000, stoppingToken); // sleep for an hour
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
