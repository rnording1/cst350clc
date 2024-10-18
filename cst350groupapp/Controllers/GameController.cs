using Microsoft.AspNetCore.Mvc;


namespace cst350groupapp.Controllers
{
    public class GameController : Controller
    {
        public ActionResult StartGame()
        {
            // Check if the session variable "User" exists and is not null
            var userJson = Session["User"] as string;
            if (string.IsNullOrEmpty(userJson))
            {
                // If the user is not logged in, redirect them to the login page
                return RedirectToAction("Login", "Account"); // Change "Account" to the correct controller handling login
            }

            // User is logged in, proceed to display the StartGame view
            return View();
        }
    }  
}
