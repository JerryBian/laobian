using System;
using System.Net.Http;
using System.Threading.Tasks;
using Laobian.Share.Setting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Share.HttpService
{
    public class BlogHttpService
    {
        private readonly BlogSetting _blogSetting;
        private readonly HttpClient _httpClient;
        private readonly ILogger<BlogHttpService> _logger;

        public BlogHttpService(HttpClient httpClient, IOptions<BlogSetting> setting, ILogger<BlogHttpService> logger)
        {
            _logger = logger;
            _blogSetting = setting.Value;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_blogSetting.BlogLocalEndpoint);
        }

        public async Task PurgeCacheAsync()
        {
            var response = await _httpClient.PostAsync("/api/PurgeCache", new StringContent(string.Empty));
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"{nameof(ApiHttpService)}.{nameof(PurgeCacheAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            }
        }
    }
}