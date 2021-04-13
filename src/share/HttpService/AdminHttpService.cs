using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share.Setting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Share.HttpService
{
    public class AdminHttpService
    {
        private readonly BlogSetting _blogSetting;
        private readonly HttpClient _httpClient;
        private readonly ILogger<BlogHttpService> _logger;

        public AdminHttpService(HttpClient httpClient, IOptions<BlogSetting> setting, ILogger<BlogHttpService> logger)
        {
            _logger = logger;
            _blogSetting = setting.Value;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_blogSetting.BlogLocalEndpoint);
        }

        public async Task UpdatePullGitFileEventAsync(bool enable)
        {
            try
            {
                var response = await _httpClient.PostAsync("/api/UpdateGitFileLogState",
                    new StringContent(enable.ToString(), Encoding.UTF8));
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        $"{nameof(AdminHttpService)}.{nameof(UpdatePullGitFileEventAsync)} failed. Status: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(AdminHttpService)}.{nameof(UpdatePullGitFileEventAsync)} failed.");
            }
        }
    }
}