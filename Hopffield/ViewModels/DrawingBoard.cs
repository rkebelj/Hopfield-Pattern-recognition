using Hopffield.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hopffield
{
    public class DrawingBoard
    {
        public List<Cell> Cells { get; private set; }

        public Command<Cell> SquareClickCommand { get; private set; }
        public int MemorySize { get;  set; }

        public DrawingBoard(int size)
        {
            MemorySize = size;
            Cells = new List<Cell>();

            for (int i = 0; i < MemorySize; i++)
            {
                for (int j = 0; j < MemorySize; j++)
                {
                    Cells.Add(new Cell() { Row = i, Column = j });
                }
            }

            SquareClickCommand = new Command<Cell>(OnSquareClick);
        }

        private void OnSquareClick(Cell square)
        {
        }
    }
    public class Cell
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public bool IsInked { get; set; } }
    
}
