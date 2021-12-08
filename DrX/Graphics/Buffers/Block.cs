using DrX.Utils;
using System;

namespace DrX.Graphics.Buffers
{
    /// <summary>
    /// Represents a Block in the BlockBuffer.<br/>
    /// It contains a matrix of characters, a background color and a foreground color.
    /// </summary>
    public class Block
    {
        public char[,] Data { get; set; }
        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }

        public char this[int i, int j]
        {
            get { return Data[i, j]; }
            set { Data[i, j] = value; }
        }

        public int Rows => Data.GetLength(0);

        public int Cols => Data.GetLength(1);

        public Block(Dimension blockSize, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black)
        {
            Data = new char[blockSize.Rows, blockSize.Cols];
            ForegroundColor = fColor;
            BackgroundColor = bColor;
            Fill(' ');
        }

        public Block(Block block)
        {
            Data = new char[block.Rows, block.Cols];
            ForegroundColor = block.ForegroundColor;
            BackgroundColor = block.BackgroundColor;
            int rowLimit = Rows;
            int colLimit = Cols;
            for (int row = 0; row < rowLimit; row++)
            {
                for (int col = 0; col < colLimit; col++)
                {
                    Data[row, col] = block[row, col];
                }
            }
        }

        public void Fill(char c)
        {
            int rowLimit = Rows;
            int colLimit = Cols;
            for (int row = 0; row < rowLimit; row++)
            {
                for (int col = 0; col < colLimit; col++)
                {
                    Data[row, col] = c;
                }
            }
        }
    }
}
