namespace cst350groupapp.Models
{
    public class Cell
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool Visited { get; set; }
        public bool Live { get; set; }
        public int LiveNeighbors { get; set; }
        public string ButtonImage { get; set; }

        public Cell() 
        {
            Row = -1;
            Column = -1;
            Visited = false;
            Live = false;
            LiveNeighbors = 0;
            ButtonImage = "";
        }

        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
            Visited = false;
            Live = false; 
            LiveNeighbors = 0;
            ButtonImage = "";
        }
    }
}