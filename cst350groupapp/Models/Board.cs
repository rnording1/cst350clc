namespace cst350groupapp.Models
{
    public class Board
    {
        public int[] Size { get; set; }
        public Cell[,] Grid { get; set; }
        public double Difficulty { get; set; }

        public Board(int[] size)
        {
            if (size.Length == 2 && size[0] > 0 && size[1] > 0)
            {
                Size = size;
            }
            //set default value if invalid input sent
            else
            {
                int[] defaultSize = new int[2];
                defaultSize[0] = 1;
                defaultSize[1] = 1;

                Size = defaultSize;

                Console.WriteLine("Warning: Invalid board size, size set to 1,1.");
            }

            Grid = SetUpGrid();

            Difficulty = 0;

        }

        //populate each location in Grid with a cell
        private Cell[,] SetUpGrid()
        {
            Grid = new Cell[Size[0], Size[1]];

            int row = 0;
            int col = 0;

            for (row = 0; row < Size[0]; row++)
            {
                for (col = 0; col < Size[1]; col++)
                {
                    Grid[row, col] = new Cell(row + 1, col + 1);
                }
            }
            

            return Grid;

        }

        public void setupLiveNeighbors()
        {
            //check if difficulty is set up right
            if(Difficulty >= 0 &&  Difficulty <= 100)
            {
                //get total amount of cells
                int totalCells = Size[0] * Size[1];
                //get amount of live cells to set
                int liveCellCount = (int)(totalCells * (Difficulty / 100.0));

                //Use a list to generate a random order of cells
                List<(int, int)> gridPositions = new List<(int, int)> ();
                for (int row = 0; row < Size[0];  row++)
                {
                    for (int col = 0; col < Size[1]; col++)
                    {
                        gridPositions.Add(new(row, col));
                    }
                }

                Random rand = new Random();
                gridPositions = gridPositions.OrderBy(_ => rand.Next()).ToList();


                //use random order to set the live value up to the number of live cells based on percentage
                for (int live = 0; live < liveCellCount; live++)
                {
                    var (row, col) = gridPositions[live];
                    Grid[row, col].Live = true;
                }

            }
            else
            {
                Console.WriteLine("Invalid difficulty Value, try again");
            }
        }

        public void calculateLiveNeighbors()
        {
            int rows = Size[0];
            int cols = Size[1];

            //loop through each cell
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int liveNeighborCount = 0;

                    //loop through the cells and rows around current cell
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            int neighborRow = row + x;
                            int neighborCol = col + y;

                            //make sure the neighbor cell is within the Grid
                            if(neighborRow >= 0 && neighborRow < rows && neighborCol >= 0 && neighborCol < cols)
                            {
                                //count the live cells
                                if (Grid[neighborRow, neighborCol].Live)
                                {
                                    liveNeighborCount++;
                                }
                            }
                        }
                    }

                    //set live neighbors to the count

                    Grid[row, col].LiveNeighbors = liveNeighborCount;

                }
            }
        }

    }
}
