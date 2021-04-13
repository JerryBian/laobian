using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.HttpService;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Admin.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly ApiHttpService _apiHttpService;
        private readonly ILogger<ApiController> _logger;

        public ApiController(ILogger<ApiController> logger, ApiHttpService apiHttpService)
        {
            _logger = logger;
            _apiHttpService = apiHttpService;
        }

        [HttpPost]
        [Route("UpdateGitFileLogState")]
        public async Task<IActionResult> UpdateGitFileLogState()
        {
            var remoteAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            var localAddress = Request.HttpContext.Connection.LocalIpAddress?.ToString();
            var isLocal = IpUtil.IsLocal(remoteAddress, localAddress);
            if (!isLocal)
            {
                return BadRequest();
            }

            using var sr = new StreamReader(Request.Body, Encoding.UTF8);
            var message = await sr.ReadToEndAsync();
            var flag = Convert.ToBoolean(message);
            if (flag)
            {
                GlobalFlag.PullGitFileEvent.Set();
            }
            else
            {
                GlobalFlag.PullGitFileEvent.Reset();
            }

            _logger.LogInformation($"UpdateGitFileLogState finished. flag = {flag}");
            return Ok();
        }
    }
}