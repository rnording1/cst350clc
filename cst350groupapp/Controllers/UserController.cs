using cst350groupapp.Models;
using Microsoft.AspNetCore.Mvc;

namespace cst350groupapp.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        public IActionResult ProcessRegister(RegisterViewModel registerViewModel)
        {
            UserModel newUser = new UserModel();
            newUser.FirstName = registerViewModel.FirstName;
            newUser.LastName = registerViewModel.LastName;
            newUser.Email = registerViewModel.Email;
            newUser.Sex = registerViewModel.Sex;
            newUser.Username = registerViewModel.Username;
            newUser.SetPassword(registerViewModel.Password);
            return View("Index");

        }
    }
}
