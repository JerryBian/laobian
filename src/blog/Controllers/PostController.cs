using System.Threading.Tasks;
using Laobian.Blog.Cache;
using Laobian.Share.HttpService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Controllers
{
    public class PostController : Controller
    {
        private readonly ApiHttpService _apiHttpService;
        private readonly ICacheClient _cacheClient;
        private readonly ILogger<PostController> _logger;

        public PostController(ICacheClient cacheClient, ApiHttpService apiHttpService, ILogger<PostController> logger)
        {
            _logger = logger;
            _cacheClient = cacheClient;
            _apiHttpService = apiHttpService;
        }

        [Route("{year}/{month}/{url}.html")]
        public async Task<IActionResult> Index([FromRoute] int year, [FromRoute] int month, [FromRoute] string url)
        {
            var viewModel = await _cacheClient.GetOrCreateAsync(
                CacheKeyBuilder.Build(nameof(PostController), nameof(Index), year, month, url),
                async () =>
                {
                    var post = await _apiHttpService.GetPostAsync(url);
                    if (post.Metadata.PublishTime.Year != year || post.Metadata.PublishTime.Month != month)
                    {
                        _logger.LogWarning(
                            $"Find post with url={url}, but either year={year} or month={month} is failed to match.");
                        return null;
                    }

                    return post;
                });

            if (viewModel == null)
            {
                return NotFound();
            }

#pragma warning disable 4014
            _apiHttpService.PostNewAccessAsync(url); // No need to wait
#pragma warning restore 4014
            return View(viewModel);
        }
    }
}