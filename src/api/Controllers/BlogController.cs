using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;
using Laobian.Share.Blog.Service;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.Controllers
{
    [ApiController]
    [Route("blog")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<BlogController> _logger;

        public BlogController(IBlogService blogService, ILogger<BlogController> logger)
        {
            _logger = logger;
            _blogService = blogService;
        }

        [HttpPost]
        [Route("reload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReloadAsync()
        {
            try
            {
                using var sr = new StreamReader(Request.Body, Encoding.UTF8);
                var message = await sr.ReadToEndAsync();
                await _blogService.FlushDataToFileAsync();
                await _blogService.PushGitFilesAsync(message);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ReloadAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("posts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BlogPost>>> GetPostsAsync()
        {
            try
            {
                var posts = _blogService.GetAllPosts();
                var result = await Task.FromResult(posts);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetPostsAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("post/{link}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BlogPost>> GetPostAsync([FromRoute] string link)
        {
            try
            {
                var post = _blogService.GetAllPosts().FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, link));
                if (post == null)
                {
                    return NotFound($"Post with link not found: {link}");
                }

                var result = await Task.FromResult(post);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetPostAsync)}({link}) failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("post/metadata")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePostMetadataAsync(BlogPostMetadata metadata)
        {
            try
            {
                var result = await _blogService.UpdatePostMetadataAsync(metadata);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UpdatePostMetadataAsync)}({JsonUtil.Serialize(metadata)}) failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("post/access/{link}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PostNewAccess([FromRoute] string link)
        {
            try
            {
                var post = _blogService.GetAllPosts().FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, link));
                if (post == null)
                {
                    _logger.LogWarning($"No post found with link {link}, new access will be discarded.");
                }
                else
                {
                    post.Access.Access.AddOrUpdate(DateTime.Now.ToString("yyyy-MM-dd"), key => 1, (key, val) =>
                    {
                        Interlocked.Increment(ref val);
                        return val;
                    });
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(PostNewAccess)}({link}) failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("tags")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BlogTag>>> GetTagsAsync()
        {
            try
            {
                var tags = _blogService.GetAllBlogTags();
                var result = await Task.FromResult(tags);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetTagsAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("tag/{link}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BlogTag>> GetTagAsync([FromRoute] string link)
        {
            try
            {
                var tags = _blogService.GetAllBlogTags();
                var tag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, link));
                if (tag == null)
                {
                    return NotFound($"Tag link = {link}");
                }

                return await Task.FromResult(tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetTagAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("tag")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BlogTag>> AddTagAsync(BlogTag tag)
        {
            try
            {
                var result = await _blogService.AddBlogTagAsync(tag);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(AddTagAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("tag")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BlogTag>> UpdateTagAsync(BlogTag tag)
        {
            try
            {
                var result = await _blogService.UpdateBlogTagAsync(tag);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UpdateTagAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Route("tag/{link}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteTagAsync([FromRoute] string link)
        {
            try
            {
                var result = await _blogService.RemoveBlogTagAsync(link);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(DeleteTagAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("comment/{postLink}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> AddCommentAsync([FromRoute] string postLink,
            [FromBody] BlogCommentItem item)
        {
            try
            {
                var result = await _blogService.AddCommentAsync(postLink, item);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(AddCommentAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("comments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BlogComment>>> GetCommentsAsync()
        {
            try
            {
                var result = await _blogService.GetCommentsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetCommentsAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("comment/{postLink}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BlogComment>>> GetCommentAsync([FromRoute] string postLink)
        {
            try
            {
                var result = await _blogService.GetCommentAsync(postLink);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetCommentAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("comment/item/{commentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BlogComment>>> GetCommentItemAsync([FromRoute] Guid commentId)
        {
            try
            {
                var result = await _blogService.GetCommentItemAsync(commentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetCommentItemAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("comment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<BlogComment>>> UpdateCommentAsync([FromBody] BlogCommentItem comment)
        {
            try
            {
                var result = await _blogService.UpdateCommentAsync(comment);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetCommentItemAsync)} failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}