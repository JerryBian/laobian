using Microsoft.AspNetCore.Mvc;

namespace Laobian.Blog.Controllers
{
    public class TagController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}