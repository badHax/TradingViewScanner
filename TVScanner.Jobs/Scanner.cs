﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using TVScanner.Shared;
using TVScanner.Shared.Configuration;
using TVScanner.Shared.Logging;
using TVScanner.Shared.Notifications;
using TVScanner.Shared.Scanner;

namespace TVScanner.Jobs
{
    public abstract class Scanner : BackgroundService
    {
        protected readonly IAbstractLogger _logger;
        protected readonly int _interval;
        private readonly ITaskDelayer _taskDelayer;
        private readonly ICache _cache;
        protected readonly IScanService _scanService;
        private readonly INotificationService _notificationService;
        protected readonly ScanFilter _scanFilter;
        private readonly string _messageType;
        private readonly IServiceScopeFactory _scopeFactory;

        protected abstract string ScannerName { get; }

        protected Scanner(
            IServiceScopeFactory scopeFactory,
            ScanFilter scanFilter,
            string messageType)
        {
            if (scopeFactory == null)
            {
                throw new ArgumentNullException(nameof(scopeFactory));
            }
            if (scanFilter == null)
            {
                throw new ArgumentNullException(nameof(scanFilter));
            }
            if (string.IsNullOrWhiteSpace(messageType))
            {
                throw new ArgumentNullException(nameof(messageType));
            }

            _scopeFactory = scopeFactory;
            var provider = _scopeFactory.CreateScope().ServiceProvider;

            _logger = provider.GetRequiredService<IAbstractLogger>();
            _cache = provider.GetRequiredService<ICache>();
            _scanService = provider.GetRequiredService<IScanService>();
            _notificationService = provider.GetRequiredService<INotificationService>();
            _interval = provider.GetRequiredService<IOptions<AppConfig>>().Value.ScannerConfig.Interval;
            _taskDelayer = provider.GetRequiredService<ITaskDelayer>();
            _messageType = messageType;
            _scanFilter = scanFilter;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = await GetScanResult();

                    if (result == null)
                    {
                        throw new WebException("No data returned from the scan service");
                    }
                    if (result.Length != 0)
                    {
                        var existing = _cache.Get<IEnumerable<ScanRecord>>(_messageType);
                        if (existing == null || !result.EqualBySymbolNames(existing))
                        {
                            var newRecords = result.Except(existing ?? Array.Empty<ScanRecord>());
                            foreach (var rec in newRecords)
                            {
                                await _notificationService.SendPushNotification(rec.Name, _scanFilter.Type);
                            }
                        }

                        using (IServiceScope scope = _scopeFactory.CreateScope())
                        {
                            IScanRecordManager scanRecordManager = scope.ServiceProvider.GetRequiredService<IScanRecordManager>();
                            await scanRecordManager.UpdateRecords(result);
                        }
                    }
                    else
                    {
                        _logger.LogInformation(this, $"No records found for {ScannerName}");
                    }

                    await _cache.Set(_messageType, result);
                }
                catch (Exception ex)
                {
                    //todo: handle exception
                    _logger.LogError(this, $"Error: {ex.Message}");
                }
                finally
                {
                    await _taskDelayer.Delay(_interval, stoppingToken);
                }
            }
        }

        protected virtual async Task<ScanRecord[]> GetScanResult()
        {
            return await _scanService.GetFilteredResult(_scanFilter);
        }
    }
}
