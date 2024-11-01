﻿using cst350groupapp.Models;

namespace cst350groupapp.Services
{
    public class BoardService
    {
        private readonly Board _board;

        public BoardService(Board board)
        {
            _board = board;
        }

        void floodFill(int row, int col)
        {
            //check for out of bounds
            if (row < 0 || row >= _board.Size[0] || col < 0 || col >= _board.Size[1] || _board.Grid[row, col].Visited)
                return;

            _board.Grid[row, col].Visited = true;

            if (_board.Grid[row, col].LiveNeighbors == 0)
            {
                floodFill(row - 1, col);
                floodFill(row + 1, col);
                floodFill(row, col - 1);
                floodFill(row, col + 1);
                floodFill(row - 1, col - 1);
                floodFill(row + 1, col - 1);
                floodFill(row + 1, col + 1);
                floodFill(row - 1, col + 1);
            }
        }

        //see if all the safe cells are visited
        private bool SafeCellsVisited()
        {
            for (int r = 0; r < _board.Size[0]; ++r)
            {
                for (int c = 0; c < _board.Size[1]; ++c)
                {
                    if (_board.Grid[r, c].Visited == false && _board.Grid[r, c].Live == false)
                    {
                        return false;
                    }
                }
            }

            return true;

        }

        //checks if all the live cells have been flagged
        private bool LiveCellsFlagged()
        {
            for (int r = 0; r < _board.Size[0]; ++r)
            {
                for (int c = 0; c < _board.Size[1]; ++c)
                {
                    if (_board.Grid[r, c].Visited == false && _board.Grid[r, c].Live == true && _board.Grid[r, c].ButtonImage != "flag.png")
                    {
                        return false;
                    }
                }
            }

            return true;

        }

        // on left click see if the button that was clicked had a bomb, game over if so. If not, flood fill.
        public void LeftClick(int row, int col)
        {
            if (_board.Grid[row, col].ButtonImage == "flag.png")
            {
                _board.Message = "Cannot left click a flagged cell";
                return;
            }
            //check if a live cell was clicked and do a game over message if so
            if (_board.Grid[row, col].Live)
            {
                _board.Grid[row, col].ButtonImage = "bomb.png";
                _board.Message = "Game Over!";
                _board.GameState = 2;
            }
            //else, check for flood fill and update the status of the buttons
            else
            {
                _board.Message = "";
                floodFill(row, col);
                if (SafeCellsVisited())
                {
                    _board.Message = "You Win!";
                    _board.GameState = 1;
                }
            }

        }

        // on right click, flag the cell
        public void RightClick(int row, int col)
        {
            //check if the cell is flagged, if so, unflag it
            if (_board.Grid[row, col].ButtonImage == "flag.png")
            {
                _board.Grid[row, col].ButtonImage = "";
            }
            //else, flag the cell
            else
            {
                _board.Grid[row, col].ButtonImage = "flag.png";
            }

        }

    }
}