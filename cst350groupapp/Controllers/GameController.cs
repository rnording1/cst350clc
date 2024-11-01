using cst350groupapp.Filters;
using cst350groupapp.Models;
using cst350groupapp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


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

            //see if there is an active game
            if(HttpContext.Session.GetObjectFromJson<Board>("CurrentGame") != null)
            {
                return RedirectToAction("PlayGame");
            }

            // User is logged in, proceed to display the StartGame view
            return View();
        }

        [SessionCheckFilter]
        public IActionResult PlayGame(StartGameViewModel startGameViewModel)
        {
            // Get player ID from session
            int playerId = HttpContext.Session.GetObjectFromJson<UserModel>("User").Id;

            var currentGame = HttpContext.Session.GetObjectFromJson<Board>("CurrentGame");
            //new game
            if (currentGame == null)
            {
                currentGame = new Board(new int[] { startGameViewModel.Rows, startGameViewModel.Columns }, startGameViewModel.Difficulty, playerId);
                HttpContext.Session.SetObjectAsJson("CurrentGame", currentGame);
            }
            //existing game, wrong player
            else if (currentGame.PlayerId != playerId)
            {
                HttpContext.Session.Remove("CurrentGame");
                return RedirectToAction("StartGame");
            }

            return View(currentGame);
        }

        [SessionCheckFilter]
        public IActionResult StartOver()
        {
            HttpContext.Session.Remove("CurrentGame");

            return View("StartGame");

        }

        [SessionCheckFilter]
        public IActionResult ButtonLeftClick(int row, int col)
        {
            var currentGame = HttpContext.Session.GetObjectFromJson<Board>("CurrentGame");
            var boardService = new BoardService(currentGame);
            boardService.LeftClick(row, col);
            HttpContext.Session.SetObjectAsJson("CurrentGame", currentGame);

            return RedirectToAction("PlayGame");
        }
    }
}
