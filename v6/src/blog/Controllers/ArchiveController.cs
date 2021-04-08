using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    public class ArchiveController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}