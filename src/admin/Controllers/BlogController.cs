using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laobian.Admin.Models;
using Laobian.Share.Blog.Model;
using Laobian.Share.HttpService;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Admin.Controllers
{
    [Route("blog")]
    public class BlogController : Controller
    {
        private readonly ApiHttpService _apiHttpService;
        private readonly BlogHttpService _blogHttpService;

        public BlogController(ApiHttpService apiHttpService, BlogHttpService blogHttpService)
        {
            _apiHttpService = apiHttpService;
            _blogHttpService = blogHttpService;
        }

        [HttpPost]
        [Route("reload")]
        public async Task<bool> ReloadAsync()
        {
            await _blogHttpService.PurgeCacheAsync();
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var message = await reader.ReadToEndAsync();
            return await _apiHttpService.ReloadBlogDataAsync(message);
        }

        [Route("posts")]
        public async Task<IActionResult> GetPostsAsync()
        {
            var posts = await _apiHttpService.GetPostsAsync();
            var tags = await _apiHttpService.GetTagsAsync();
            var viewModel = new PostsViewModel
                {Posts = posts.OrderByDescending(x => x.Metadata.PublishTime).ToList(), Tags = tags};
            return View("Posts", viewModel);
        }

        [Route("post/{link}")]
        public async Task<BlogPost> GetPostAsync([FromRoute] string link)
        {
            var post = await _apiHttpService.GetPostAsync(link);
            return post;
        }

        [HttpPost]
        [Route("post/metadata")]
        public async Task<bool> UpdatePostMetadataAsync([FromBody] BlogPostMetadata metadata)
        {
            var result = await _apiHttpService.UpdatePostMetadataAsync(metadata);
            return result;
        }

        [Route("tags")]
        public async Task<IActionResult> GetTagsAsync()
        {
            var tags = await _apiHttpService.GetTagsAsync();
            return View("tags", tags);
        }

        [Route("tag/{link}")]
        public async Task<BlogTag> GetTagAsync(string link)
        {
            var tag = await _apiHttpService.GetTagAsync(link);
            return tag;
        }

        [HttpDelete]
        [Route("tag/{link}")]
        public async Task<bool> DeleteTagAsync(string link)
        {
            var result = await _apiHttpService.DeleteTagAsync(link);
            return result;
        }

        [HttpPut]
        [Route("tag")]
        public async Task<bool> AddTagAsync([FromBody] BlogTag tag)
        {
            var result = await _apiHttpService.AddTagAsync(tag);
            return result;
        }

        [HttpPost]
        [Route("tag")]
        public async Task<bool> UpdateTagAsync([FromBody] BlogTag tag)
        {
            var result = await _apiHttpService.UpdateTagAsync(tag);
            return result;
        }

        [HttpGet]
        [Route("comments")]
        public async Task<IActionResult> GetCommentsAsync([FromQuery] string postLink, [FromQuery] string ip,
            [FromQuery] bool? isPublished, [FromQuery] bool? isReviewed, [FromQuery] string userName)
        {
            var viewModel = new List<CommentsViewModel>();
            if (string.IsNullOrEmpty(postLink))
            {
                var posts = await _apiHttpService.GetPostsAsync();
                foreach (var blogPost in posts)
                {
                    foreach (var comment in blogPost.Comment.CommentItems)
                    {
                        viewModel.Add(new CommentsViewModel
                        {
                            Comment = comment,
                            Post = blogPost
                        });
                    }
                }
            }
            else
            {
                var post = await _apiHttpService.GetPostAsync(postLink);
                if (post != null)
                {
                    foreach (var comment in post.Comment.CommentItems)
                    {
                        viewModel.Add(new CommentsViewModel
                        {
                            Comment = comment,
                            Post = post
                        });
                    }
                }
            }

            if (!string.IsNullOrEmpty(ip))
            {
                viewModel = viewModel.Where(x => x.Comment.Ip == ip).ToList();
            }

            if (isPublished.HasValue)
            {
                viewModel = viewModel.Where(x => x.Comment.IsPublished == isPublished.Value).ToList();
            }

            if (isReviewed.HasValue)
            {
                viewModel = viewModel.Where(x => x.Comment.IsReviewed == isReviewed.Value).ToList();
            }

            if (!string.IsNullOrEmpty(userName))
            {
                viewModel = viewModel.Where(x => x.Comment.UserName == userName).ToList();
            }

            return View("Comments", viewModel.OrderByDescending(x => x.Comment.TimeStamp).ToList());
        }

        [HttpGet]
        [Route("comment/{postLink}")]
        public async Task<ActionResult<BlogComment>> GetCommentAsync([FromRoute] string postLink)
        {
            var result = await _apiHttpService.GetCommentAsync(postLink);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("comment/item/{id}")]
        public async Task<ActionResult<CommentsViewModel>> GetCommentAsync([FromRoute] Guid id)
        {
            var result = await _apiHttpService.GetCommentItemAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("comment")]
        public async Task<bool> UpdateCommentAsync([FromBody] BlogCommentItem comment)
        {
            var result = await _apiHttpService.UpdateCommentAsync(comment);
            return result;
        }
    }
}