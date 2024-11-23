using cst350groupapp.Filters;
using cst350groupapp.Models;
using cst350groupapp.Services;
using Glimpse.Mvc.AlternateType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace cst350groupapp.Controllers
{
    public class GameController : Controller
    {
        private readonly ICompositeViewEngine _viewEngine;
        private readonly GameDAO gameDAO;

        public GameController(ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
            gameDAO = new GameDAO();
        }

        [SessionCheckFilter]
        public IActionResult InitializeGame(StartGameViewModel startGameViewModel)
        {
            // Get player ID from session
            int playerId = HttpContext.Session.GetObjectFromJson<UserModel>("User").Id;

            // Create a new Board with the user's settings
            var currentGame = new Board(new int[] { startGameViewModel.Rows, startGameViewModel.Columns }, startGameViewModel.Difficulty, playerId);
            HttpContext.Session.SetObjectAsJson("CurrentGame", currentGame);

            // Serialize the Board object to a JSON string
            string gameData = System.Text.Json.JsonSerializer.Serialize(currentGame);

            // Create a GameModel object
            GameModel gameModel = new GameModel
            {
                PlayerId = playerId,
                DateSaved = DateTime.Now,
                GameData = gameData
            };

            // Save the game to the database and get the generated Id
            GameDAO gameDAO = new GameDAO();
            int gameId = gameDAO.SaveGame(gameModel);

            // Assign the generated Id to the Board object
            currentGame.Id = gameId;

            // Save the Board object to the session
            HttpContext.Session.SetObjectAsJson("CurrentGame", currentGame);

            // Redirect to PlayGame to display the initialized game board
            return RedirectToAction("PlayGame");
        }


        [SessionCheckFilter]
        public IActionResult StartGame()
        {
            var currentGame = HttpContext.Session.GetObjectFromJson<Board>("CurrentGame");

            // Check if the session variable "User" exists and is not null
            var userJson = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userJson))
            {
                // If the user is not logged in, redirect them to the login page
                return RedirectToAction("Login", "User"); // Change "UserController" to "User"
            }

            //see if there is an active game
            if(currentGame != null)
            {
                if (currentGame.GameState == 1 || currentGame.GameState == 2) // Win or Loss
                {
                    return RedirectToAction("EndGame");
                }
                return RedirectToAction("PlayGame");
            }

            // User is logged in, proceed to display the StartGame view
            return View();
        }

        [SessionCheckFilter]
        public IActionResult PlayGame()
        {
            var currentGame = HttpContext.Session.GetObjectFromJson<Board>("CurrentGame");
            int playerId = HttpContext.Session.GetObjectFromJson<UserModel>("User").Id;

            //if the logged in user is not the same as the player id of the current game then redirect to start game
            if (currentGame != null && currentGame.PlayerId != playerId)
            {
                return RedirectToAction("StartGame");
            }
            else if (currentGame == null)
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

            gameDAO.DeleteGame(createGameModel());

            if (currentGame != null)
            {
                if (currentGame.GameState == 1) // Win
                {
                    return RedirectToAction("WinPage");
                }
                else if (currentGame.GameState == 2) // Loss
                {
                    return RedirectToAction("LosePage");
                }
            }

            // Fallback: if the game is still in progress or something went wrong, return to PlayGame
            return RedirectToAction("PlayGame");
        }


        [SessionCheckFilter]
        public IActionResult StartOver()
        {
            gameDAO.DeleteGame(createGameModel());

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

            // Check game state and handle accordingly
            if (currentGame.GameState != 0)
            {
                // Game over, redirect to EndGame
                return Json(new { isGameOver = true, redirectUrl = Url.Action("EndGame") });
            }

            // Return the updated grid as a partial view
            return PartialView("_GameGrid", currentGame);
        }

        [SessionCheckFilter]
        public IActionResult ButtonRightClick(int row, int col)
        {
            var currentGame = HttpContext.Session.GetObjectFromJson<Board>("CurrentGame");
            var boardService = new BoardService(currentGame);

            // Perform the left click on the specified cell
            boardService.RightClick(row, col);

            // Update the game state in the session
            HttpContext.Session.SetObjectAsJson("CurrentGame", currentGame);

            if (currentGame.GameState != 0)
            {
                // Game over, redirect to EndGame
                return Json(new { isGameOver = true, redirectUrl = Url.Action("EndGame") });
            }

            // Return the updated grid as a partial view
            return PartialView("_GameGrid", currentGame);
        }

        public IActionResult WinPage()
        {
            var currentGame = HttpContext.Session.GetObjectFromJson<Board>("CurrentGame");
            
            return View(currentGame);
        }

        public IActionResult LosePage()
        {
            var currentGame = HttpContext.Session.GetObjectFromJson<Board>("CurrentGame");
            return View(currentGame);
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }


        [HttpPost]
        [SessionCheckFilter]
        public IActionResult SaveGame()
        {
            // Retrieve the current game board from the session
            Board currentGame = HttpContext.Session.GetObjectFromJson<Board>("CurrentGame");

            if (currentGame != null)
            {
                // Update the existing game in the database
                gameDAO.UpdateGame(createGameModel());
                currentGame.Message = "Game updated successfully!";

                // Save the updated board back to the session
                HttpContext.Session.SetObjectAsJson("CurrentGame", currentGame);
            }
            else
            {
                return RedirectToAction("StartGame");
            }

            // Return the PlayGame view with the current board
            return RedirectToAction("PlayGame");
        }

        [SessionCheckFilter]
        public IActionResult ViewGames()
        {
            // Get the current user
            var user = HttpContext.Session.GetObjectFromJson<UserModel>("User");
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Get the list of games for this user
            var games = gameDAO.GetAllGames(user.Id);

            List<GameViewModel> gameViewModels = new List<GameViewModel>();

            // Deserialize GameData and extract properties
            foreach (var game in games)
            {
                if (!string.IsNullOrEmpty(game.GameData))
                {
                    try
                    {
                        var board = System.Text.Json.JsonSerializer.Deserialize<Board>(game.GameData);
                        if (board != null)
                        {
                            GameViewModel gameViewModel = new GameViewModel(game, board.Size[0], board.Size[1], board.Difficulty);
                            gameViewModels.Add(gameViewModel);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            return View(gameViewModels);
        }

        public IActionResult LoadGame(int id)
        {
            // Get the current user
            var user = HttpContext.Session.GetObjectFromJson<UserModel>("User");
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Get the game from the database
            var game = gameDAO.GetGameById(id, user.Id);

            // Deserialize the game data
            var board = System.Text.Json.JsonSerializer.Deserialize<Board>(game.GameData);

            // Save the game to the session
            HttpContext.Session.SetObjectAsJson("CurrentGame", board);

            // Redirect to the PlayGame view
            return RedirectToAction("PlayGame");
        }

        public IActionResult DeleteGame (int id) {
            // Get the current user
            var user = HttpContext.Session.GetObjectFromJson<UserModel>("User");
            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Get the game from the database
            var game = gameDAO.GetGameById(id, user.Id);

            // Delete the game from the database
            gameDAO.DeleteGame(game);

            // Redirect to the ViewGames view
            return RedirectToAction("ViewGames");
        }

        [SessionCheckFilter]
        private GameModel createGameModel()
        {

            var currentGame = HttpContext.Session.GetObjectFromJson<Board>("CurrentGame");

            // Get the current user ID
            string? userJson = HttpContext.Session.GetString("User");
            UserModel? user = userJson != null ? System.Text.Json.JsonSerializer.Deserialize<UserModel>(userJson) : null;
            int playerId = user?.Id ?? 0;

            string gameData = System.Text.Json.JsonSerializer.Serialize(currentGame);

            GameModel game = new GameModel
            {
                Id = currentGame.Id, // Use the GameId from the Board
                PlayerId = playerId,
                DateSaved = DateTime.Now,
                GameData = gameData
            };

            return game;

        }

    }

}
