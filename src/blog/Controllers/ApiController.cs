using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<ApiController> _logger;

        public ApiController(ILogger<ApiController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("PurgeCache")]
        public IActionResult PurgeCache()
        {
            _logger.LogInformation($"PurgeCache <= {Request.HttpContext.Connection.RemoteIpAddress}");
            _logger.LogInformation($"PurgeCache <= {Request.HttpContext.Connection.LocalIpAddress}");
            _logger.LogInformation($"PurgeCache <= {Request.Host.Value.StartsWith("localhost:")}");
            GlobalFlag.HardRefreshAt = DateTime.Now;
            return Ok();
        }
    }
}