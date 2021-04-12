using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Blog.Cache;
using Laobian.Blog.Models;
using Laobian.Share.Blog.Model;
using Laobian.Share.HttpService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Controllers
{
    public class TagController : Controller
    {
        private readonly ApiHttpService _apiHttpService;
        private readonly ICacheClient _cacheClient;
        private readonly ILogger<TagController> _logger;

        public TagController(ICacheClient cacheClient, ApiHttpService apiHttpService, ILogger<TagController> logger)
        {
            _logger = logger;
            _cacheClient = cacheClient;
            _apiHttpService = apiHttpService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _cacheClient.GetOrCreateAsync(
                CacheKeyBuilder.Build(nameof(TagController), nameof(Index)),
                async () =>
                {
                    var posts = await _apiHttpService.GetPostsAsync();
                    var tags = posts.SelectMany(x => x.Tags).Distinct().ToList();
                    var result = new List<TagViewModel>();
                    foreach (var blogTag in tags)
                    {
                        var tagViewModel = new TagViewModel
                            {Key = blogTag.Name, Tag = blogTag, Posts = new List<BlogPost>()};
                        foreach (var blogPost in posts)
                        {
                            if (blogPost.Tags.Contains(blogTag))
                            {
                                tagViewModel.Posts.Add(blogPost);
                            }
                        }

                        tagViewModel.Posts = tagViewModel.Posts.OrderByDescending(x => x.Metadata.PublishTime).ToList();
                        result.Add(tagViewModel);
                    }

                    return result;
                });

            return View(viewModel);
        }
    }
}