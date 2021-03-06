﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Blog.Alert;
using Laobian.Share.Log;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.HostedService
{
    public class LogHostedService : BackgroundService
    {
        private readonly IBlogAlertService _alertService;
        private readonly ILogger<LogHostedService> _logger;
        private DateTime _lastReportGeneratedAt;

        public LogHostedService(
            IBlogAlertService alertService,
            ILogger<LogHostedService> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (Global.Environment.IsDevelopment())
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    await GenerateLogReportAsync();
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);
                }

                try
                {
                    if (DateTime.Now.Date > _lastReportGeneratedAt.Date &&
                        DateTime.Now.Hour == Global.Config.Blog.LogFlushAtHour)
                    {
                        await GenerateLogReportAsync();
                        _lastReportGeneratedAt = DateTime.Now;
                    }

                    _logger.LogDebug("Logs flushed completely.");
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex, "[Need Attention] - Log hosted service failed.");
                }
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(LogHostedService)} is starting.");
            await base.StartAsync(cancellationToken);
            _logger.LogInformation($"{nameof(LogHostedService)} has started.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(LogHostedService)} is stopping.");
            await GenerateLogReportAsync();
            await base.StopAsync(cancellationToken);
            _logger.LogInformation($"{nameof(LogHostedService)} has stopped.");
        }

        private async Task GenerateLogReportAsync()
        {
            var logs = new List<LogEntry>();
            while (Global.InMemoryLogQueue.TryDequeue(out var item))
            {
                if (item.Level >= LogLevel.Warning)
                {
                    logs.Add(item);
                }
            }

            await _alertService.AlertLogsAsync(logs);
        }
    }
}