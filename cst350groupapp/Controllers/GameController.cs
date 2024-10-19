using cst350groupapp.Filters;
using Microsoft.AspNetCore.Mvc;


namespace cst350groupapp.Controllers
{
    public class GameController : Controller
    {
        [SessionCheckFilter]
        public IActionResult StartGame()
        {
            // Check if the session variable "User" exists and is not null
            var userJson = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userJson))
            {
                // If the user is not logged in, redirect them to the login page
                return RedirectToAction("Login", "User"); // Change "UserController" to "User"
            }

            // User is logged in, proceed to display the StartGame view
            return View();
        }
    }
}
