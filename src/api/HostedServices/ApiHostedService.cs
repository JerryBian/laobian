using System;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Blog.Service;
using Laobian.Share.HttpService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.HostedServices
{
    public class ApiHostedService : BackgroundService
    {
        private readonly IBlogService _blogService;
        private readonly BlogHttpService _blogHttpService;
        private readonly AdminHttpService _adminHttpService;
        private readonly ILogger<ApiHostedService> _logger;

        public ApiHostedService(IBlogService blogService, BlogHttpService blogHttpService, AdminHttpService adminHttpService, ILogger<ApiHostedService> logger)
        {
            _logger = logger;
            _blogHttpService = blogHttpService;
            _adminHttpService = adminHttpService;
            _blogService = blogService;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
            await _blogService.InitDataFromFileAsync();
            GlobalFlag.PullGitFileEvent.Set();
            await Task.WhenAll(_blogHttpService.UpdatePullGitFileEventAsync(true),
                _adminHttpService.UpdatePullGitFileEventAsync(true));
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(_blogHttpService.UpdatePullGitFileEventAsync(false),
                _adminHttpService.UpdatePullGitFileEventAsync(false));
            await _blogService.PushGitFilesAsync("Api server ending");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                await _blogService.FlushDataToFileAsync();
                await _blogService.PushGitFilesAsync("Updating by Api server");
            }
        }
    }
}