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
        public IActionResult InitializeGame(StartGameViewModel startGameViewModel)
        {
            // Get player ID from session
            int playerId = HttpContext.Session.GetObjectFromJson<UserModel>("User").Id;

            // Calculate the number of mines based on difficulty percentage
            int totalCells = startGameViewModel.Rows * startGameViewModel.Columns;
            int numberOfMines = (int)(totalCells * (startGameViewModel.Difficulty / 100));

            // Create a new Board with the user's settings
            var currentGame = new Board(new int[] { startGameViewModel.Rows, startGameViewModel.Columns }, numberOfMines, playerId);
            HttpContext.Session.SetObjectAsJson("CurrentGame", currentGame);

            // Redirect to PlayGame to display the initialized game board
            return RedirectToAction("PlayGame");
        }


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
        public IActionResult PlayGame()
        {
            var currentGame = HttpContext.Session.GetObjectFromJson<Board>("CurrentGame");

            if (currentGame == null)
            {
                // If there's no game in session, redirect to StartGame
                return RedirectToAction("StartGame");
            }

            return View(currentGame);
        }


        [SessionCheckFilter]
        public IActionResult EndGame()
        {
            var currentGame = HttpContext.Session.GetObjectFromJson<Board>("CurrentGame");

            if (currentGame != null)
            {
                if (currentGame.GameState == 1) // Win
                {
                    return RedirectToAction("WinPage");
                }
                else if (currentGame.GameState == 2) // Loss
                {
                    return RedirectToAction("LossPage");
                }
            }

            // Fallback: if the game is still in progress or something went wrong, return to PlayGame
            return RedirectToAction("PlayGame");
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

            // Perform the left click on the specified cell
            boardService.LeftClick(row, col);

            // Update the game state in the session
            HttpContext.Session.SetObjectAsJson("CurrentGame", currentGame);

            // Check if the game is over
            if (currentGame.GameState == 1) // GameState == 1 means Win
            {
                return RedirectToAction("WinPage");
            }
            else if (currentGame.GameState == 2) // GameState == 2 means Loss
            {
                return RedirectToAction("LossPage");
            }

            // Continue playing if the game is still ongoing
            return RedirectToAction("PlayGame");
        }

        public IActionResult WinPage()
        {
            return View("WinPage");
        }

        public IActionResult LosePage()
        {
            return View("LossPage");
        }





    }
}
