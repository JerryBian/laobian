using System;
using System.Threading.Tasks;
using Laobian.Share.Blog.Model;
using Laobian.Share.HttpService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<ApiController> _logger;
        private readonly ApiHttpService _apiHttpService;

        public ApiController(ILogger<ApiController> logger, ApiHttpService apiHttpService)
        {
            _logger = logger;
            _apiHttpService = apiHttpService;
        }

        [HttpPost]
        [Route("PurgeCache")]
        public IActionResult PurgeCache()
        {
            _logger.LogInformation($"PurgeCache <= {Request.HttpContext.Connection.RemoteIpAddress}");
            _logger.LogInformation($"PurgeCache <= {Request.HttpContext.Connection.LocalIpAddress}");
            _logger.LogInformation($"PurgeCache <= {Request.Host.Value}");
            GlobalFlag.HardRefreshAt = DateTime.Now;
            return Ok();
        }

        [HttpPost]
        [Route("comment")]
        public async Task<IActionResult> AddComment([FromBody] BlogCommentItem comment, [FromQuery] string postLink)
        {
            comment.IsAdmin = User.Identity?.IsAuthenticated ?? false;
            comment.Ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _apiHttpService.AddCommentAsync(postLink, comment);
            if (result)
            {
                GlobalFlag.HardRefreshAt = DateTime.Now;
                return Ok();
            }

            return BadRequest();
        }
    }
}