using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Blog.Cache;
using Laobian.Blog.Models;
using Laobian.Share.HttpService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.Controllers
{
    public class ArchiveController : Controller
    {
        private readonly ApiHttpService _apiHttpService;
        private readonly ICacheClient _cacheClient;
        private readonly ILogger<ArchiveController> _logger;

        public ArchiveController(ICacheClient cacheClient, ApiHttpService apiHttpService, ILogger<ArchiveController> logger)
        {
            _logger = logger;
            _cacheClient = cacheClient;
            _apiHttpService = apiHttpService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _cacheClient.GetOrCreateAsync(
                CacheKeyBuilder.Build(nameof(ArchiveController), nameof(Index)),
                async () =>
                {
                    var posts = await _apiHttpService.GetPostsAsync();
                    var groupedPosts = posts.GroupBy(x => x.Metadata.PublishTime.ToString("yyyy"))
                        .OrderByDescending(x => x.Key);
                    var result = new List<ArchiveViewModel>();
                    foreach (var groupedPost in groupedPosts)
                    {
                        result.Add(new ArchiveViewModel
                        {
                            DateKey = groupedPost.Key,
                            Posts = groupedPost.ToList()
                        });
                    }

                    return result;
                });
            return View(viewModel);
        }
    }
}