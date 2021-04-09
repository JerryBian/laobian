using System.Linq;
using System.Threading.Tasks;
using Laobian.Blog.Cache;
using Laobian.Blog.Models;
using Laobian.Share.Extension;
using Laobian.Share.HttpService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiHttpService _apiHttpService;
        private readonly ICacheClient _cacheClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ICacheClient cacheClient, ILogger<HomeController> logger,
            ApiHttpService apiHttpService)
        {
            _logger = logger;
            _cacheClient = cacheClient;
            _apiHttpService = apiHttpService;
        }

        public async Task<IActionResult> Index([FromQuery] int p)
        {
            var viewModel = await _cacheClient.GetOrCreateAsync(
                CacheKeyBuilder.Build(nameof(HomeController), nameof(Index), p), async () =>
                {
                    var posts = await _apiHttpService.GetPostsAsync();
                    posts = posts.Where(x => x.IsPostPublished()).OrderByDescending(x => x.Metadata.PublishTime)
                        .ToList();
                    var model = new PagedPostIndexViewModel(p, posts.Count(), 8) {Url = Request.Path};
                    foreach (var post in posts.ToPaged(8, model.CurrentPage))
                    {
                        var postIndexModel = new PostIndexViewModel
                        {
                            Link = post.GetFullLink(), PublishTime = post.GetPublishTimeString(),
                            Title = post.Metadata.Title,
                            Tags = post.Tags,
                            Access = post.GetAccessCountString(),
                            Comments = post.GetCommentCount(),
                            Excerpt = post.GetExcerpt()
                        };

                        model.Posts.Add(postIndexModel);
                    }

                    return model;
                });

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}