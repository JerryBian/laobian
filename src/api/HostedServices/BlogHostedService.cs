using System;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Blog.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.HostedServices
{
    public class BlogHostedService : BackgroundService
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<BlogHostedService> _logger;

        public BlogHostedService(IBlogService blogService, ILogger<BlogHostedService> logger)
        {
            _logger = logger;
            _blogService = blogService;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
            await _blogService.ReloadAsync();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _blogService.PushGitFilesAsync("Api server ending");
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                await _blogService.FlushDataAsync();
                await _blogService.PushGitFilesAsync("Updating by Api server");
            }
        }
    }
}