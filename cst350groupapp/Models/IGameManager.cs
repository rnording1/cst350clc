namespace cst350groupapp.Models
{
    public interface IGameManager
    {
        public List<GameModel> GetAllGames(int playerId);
        public GameModel GetGameById(int id, int playerId);
        public int SaveGame(GameModel game);
        public void DeleteGame(GameModel game);
        public void UpdateGame(GameModel game);
        public int CheckCredentials(string username, string password);
    }
}
