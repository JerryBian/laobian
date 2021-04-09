using System;
using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        [HttpPost]
        [Route("PurgeCache")]
        public IActionResult PurgeCache()
        {
            GlobalFlag.HardRefreshAt = DateTime.Now;
            return Ok();
        }
    }
}