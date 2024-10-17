using cst350groupapp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace cst350groupapp.Controllers
{
    public class UserController : Controller
    {
        UsersDAO users = new UsersDAO();
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
            newUser.Age = registerViewModel.Age;

            if (!IsValidPassword(registerViewModel.Password))
            {
                ModelState.AddModelError("Password", "Password must be 8 characters long and contain at least 1 uppercase letter, 1 lowercase letter, and 1 number");
                return View("Register", registerViewModel);
            }

            if (users.AddUser(newUser) != -1)
            {
                return View("RegisterSuccess");
            }
            else
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View("Register", registerViewModel);
            }
        }

        private bool IsValidPassword(string password)
        {
            // Check if password is at least 8 characters long
            if (password.Length < 8)
            {
                return false;
            }

            // Check if password contains at least 1 uppercase letter, 1 lowercase letter, and 1 number
            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$"))
            {
                return false;
            }

            return true;
        }
    }
}
