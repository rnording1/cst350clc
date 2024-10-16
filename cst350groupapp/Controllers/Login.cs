using Microsoft.AspNetCore.Mvc;

namespace cst350groupapp.Controllers
{
    public class Login : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
