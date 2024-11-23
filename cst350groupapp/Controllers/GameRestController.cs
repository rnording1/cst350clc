using cst350groupapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace cst350groupapp.Controllers
{
    [ApiController]
    [Route("api/showSavedGames")]
    public class GameRestController : ControllerBase
    {
        private readonly ICompositeViewEngine _viewEngine;
        private readonly GameDAO gameDAO;

        public GameRestController(ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
            gameDAO = new GameDAO();
        }


        [HttpGet("{playerId}")]
        public ActionResult<IEnumerable<GameModel>> ShowSavedGames(int playerId)
        {
            IEnumerable<GameModel> games = gameDAO.GetAllGames(playerId);
            return Ok(games);
        }

        [HttpGet("{playerId}/{gameId}")]
        public ActionResult<GameModel> ShowSavedGame(int playerId, int gameId)
        {
            GameModel game = gameDAO.GetGameById(gameId, playerId);
            return Ok(game);
        }

        [HttpDelete("{playerId}/{gameId}")]
        public ActionResult DeleteOneGame(int playerId, int gameId)
        {
            GameModel game = gameDAO.GetGameById(gameId, playerId);
            gameDAO.DeleteGame(game);
            return Ok();
        }

    }
}
