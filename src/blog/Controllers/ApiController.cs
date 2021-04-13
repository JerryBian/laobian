﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Laobian.Share;
using Laobian.Share.Blog.Model;
using Laobian.Share.HttpService;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Controllers
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
        [Route("PurgeCache")]
        public IActionResult PurgeCache()
        {
            var remoteAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            var localAddress = Request.HttpContext.Connection.LocalIpAddress?.ToString();
            var isLocal = IpUtil.IsLocal(remoteAddress, localAddress);
            if (!isLocal)
            {
                return BadRequest();
            }

            GlobalFlag.HardRefreshAt = DateTime.Now;
            return Ok();
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