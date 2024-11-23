namespace cst350groupapp.Models
{
    public class GameViewModel
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public DateTime DateSaved { get; set; }
        public string GameData { get; set; }

        // New properties for display
        public int Rows { get; set; }
        public int Columns { get; set; }
        public double Difficulty { get; set; }

        public GameViewModel(GameModel game, int row, int col, double dif)
        {
            Id = game.Id;
            PlayerId = game.PlayerId;
            DateSaved = game.DateSaved;
            GameData = game.GameData;

            // New properties for display
            Rows = row;
            Columns = col;
            Difficulty = dif;
        }

    }

}
