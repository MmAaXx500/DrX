using DrX.Utils;
using System;

namespace DrX.Graphics.Buffers
{
    /// <summary>
    /// Represents the screen char by char with colors associated to them
    /// </summary>
    public class ScreenBuffer
    {
        public ColoredChar[,] Chars { get; private set; }

        public ColoredChar this[int i, int j]
        {
            get { return Chars[i, j]; }
            set { Chars[i, j] = value; }
        }

        public int Rows => Chars.GetLength(0);

        public int Cols => Chars.GetLength(1);

        public ScreenBuffer(Dimension size)
        {
            Chars = new ColoredChar[size.Rows, size.Cols];
            Fill(' ');
        }

        public ScreenBuffer(ScreenBuffer screenBuffer)
        {
            Chars = new ColoredChar[screenBuffer.Rows, screenBuffer.Cols];

            int rowLim = Rows;
            int colLim = Cols;
            for (int row = 0; row < rowLim; row++)
            {
                for (int col = 0; col < colLim; col++)
                {
                    Chars[row, col] = new(screenBuffer[row, col]);
                }
            }
        }

        /// <summary>
        /// Fills the whole buffer with the <paramref name="c"/> character and with the optionally specified colors
        /// </summary>
        /// <param name="c"></param>
        /// <param name="fcolor"></param>
        /// <param name="bcolor"></param>
        public void Fill(char c, ConsoleColor fcolor = ConsoleColor.White, ConsoleColor bcolor = ConsoleColor.Black)
        {
            int rowLim = Rows;
            int colLim = Cols;
            for (int row = 0; row < rowLim; row++)
            {
                for (int col = 0; col < colLim; col++)
                {
                    Chars[row, col] = new(c, fcolor, bcolor);
                }
            }
        }

        /// <summary>
        /// Determines if the buffer contains any character with non default colors
        /// </summary>
        /// <returns>True if contains colored characters. False otherwise</returns>
        public bool IsColored()
        {
            int rowLim = Rows;
            int colLim = Cols;
            for (int row = 0; row < rowLim; row++)
            {
                for (int col = 0; col < colLim; col++)
                {
                    if (Chars[row, col].ForegroundColor != ConsoleColor.White 
                        || Chars[row, col].BackgroundColor != ConsoleColor.Black)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Converts the whole buffer to string without color information.
        /// </summary>
        /// <returns>String representation of the buffer without any color information</returns>
        public string GetAsString()
        {
            string ret = "";
            int rowLim = Rows;
            int colLim = Cols;
            for (int row = 0; row < rowLim; row++)
            {
                for (int col = 0; col < colLim; col++)
                {
                    ret += Chars[row, col].Char;
                }
                if (row < rowLim - 1)
                    ret += "\n";
            }
            return ret;
        }

        /// <summary>
        /// Returns the buffer a string lines
        /// </summary>
        /// <returns>An array with the contained lines</returns>
        public string[] GetAsLines()
        {
            string[] ret = new string[Rows];
            int rowLim = Rows;
            int colLim = Cols;
            for (int row = 0; row < rowLim; row++)
            {
                string line = "";
                for (int col = 0; col < colLim; col++)
                {
                    line += Chars[row, col].Char;
                }
                ret[row] = line;
            }
            return ret;
        }

        public override bool Equals(object obj)
        {
            if (obj is ScreenBuffer other)
            {
                if (other.Rows == Rows && other.Cols == Cols)
                {
                    int rowLim = Rows;
                    int colLim = Cols;
                    for (int row = 0; row < rowLim; row++)
                    {
                        for (int col = 0; col < colLim; col++)
                        {
                            if (Chars[row, col].Equals(other.Chars[row, col]) == false)
                                return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Rows ^ Cols;
        }
    }
}
