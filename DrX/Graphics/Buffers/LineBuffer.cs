using DrX.Utils;
using DrX.Utils.Generic;
using System;

namespace DrX.Graphics.Buffers
{
    /// <summary>
    /// Represents a LineBuffer which can contains multiple lines of text.<br/>
    /// It can determine it's required row number based on the screen dimensions.<br/>
    /// It also supports color formatted string variables. see: <see cref="LineBuffer(string)"/>
    /// </summary>
    public class LineBuffer
    {
        private ColoredChar[][] Lines;

        public ColoredChar this[int i, int j]
        {
            get => Lines[i][j];
            set => Lines[i][j] = value;
        }

        public int Rows => Lines.Length;

        public int Cols(int row)
        {
            return Lines[row].Length;
        }

        public LineBuffer(int rows)
        {
            Lines = new ColoredChar[rows][];
            for (int i = 0; i < Lines.Length; i++)
            {
                Lines[i] = new ColoredChar[1];
            }
        }

        public LineBuffer(LineBuffer lineBuffer)
        {
            Lines = new ColoredChar[lineBuffer.Lines.Length][];
            for (int row = 0; row < Lines.Length; row++)
            {
                Lines[row] = new ColoredChar[lineBuffer.Lines[row].Length];
                for (int col = 0; col < Lines[row].Length; col++)
                {
                    Lines[row][col] = lineBuffer.Lines[row][col];
                }
            }
        }

        /// <summary>
        /// A constructor that accepts a special string that can contain color information.<br/>
        /// The format as follows:
        /// <code>normal text °b12text with red backkground°b-- °f09 text with blue foreground °f--</code>
        /// Where the formatting started with a "<c>°</c>"
        /// followed by an "<c>b</c>" for background or an "<c>f</c>" for foreground color
        /// and two zero padded digits which refers to <see cref="ConsoleColor"/> values<br/>
        /// <br/>
        /// <c>°b12</c> -> red background color<br/>
        /// <c>°f09</c> -> blue foreground color<br/>
        /// <c>°b--</c> -> undo the last background color change<br/>
        /// <c>°f--</c> -> undo the last foreground color change<br/>
        /// </summary>
        /// <param name="coloredString"></param>
        public LineBuffer(string coloredString)
        {
            ParseColoredStr(coloredString);
        }

        /// <summary>
        /// Sets a whole line of text at <paramref name="idx"/>
        /// </summary>
        /// <param name="idx">Line index</param>
        /// <param name="line">The line</param>
        public void SetLine(int idx, ColoredChar[] line)
        {
            Lines[idx] = line;
        }

        /// <summary>
        /// Calculate the required rows at the specified window dimensions
        /// </summary>
        /// <param name="dimension">Window dimension</param>
        /// <returns>Required rows at the specified window dimensions.</returns>
        public int RequiredRowsAtDimension(Dimension dimension)
        {
            int reqRows = 0;
            for (int i = 0; i < Rows; i++)
            {
                reqRows += (Cols(i) - 1) / dimension.Cols + 1; // integer division with rounded up result. Returns at least 1.
            }
            return reqRows;
        }

        /// <summary>
        /// Parse the colored string. For more information see: <see cref="LineBuffer(string)"/>
        /// </summary>
        /// <param name="coloredString"></param>
        private void ParseColoredStr(string coloredString)
        {
            List<List<ColoredChar>> cChars = new();
            cChars.Add(new());

            LinkedList<ConsoleColor> fgColorStack = new();
            fgColorStack.PushBack(ConsoleColor.White);

            LinkedList<ConsoleColor> bgColorStack = new();
            bgColorStack.PushBack(ConsoleColor.Black);

            int ccLine = 0;
            int readIDx = 0;
            while (readIDx < coloredString.Length)
            {
                if (coloredString[readIDx] == '°')
                {
                    string colorType = coloredString.Substring(readIDx + 1, 1);
                    string colorCode = coloredString.Substring(readIDx + 2, 2);

                    bool foreground = true;
                    if (colorType == "b")
                        foreground = false;

                    if (colorCode == "--") // end of color
                    {
                        if (foreground)
                            fgColorStack.PopBack();
                        else
                            bgColorStack.PopBack();
                    }
                    else
                    {
                        if (foreground)
                            fgColorStack.PushBack(ParseEscapeCode(colorCode));
                        else
                            bgColorStack.PushBack(ParseEscapeCode(colorCode));
                    }

                    readIDx += 3; //  colorType + colorCode
                }
                else if (coloredString[readIDx] == '\n')
                {
                    cChars.Add(new());
                    ccLine++;
                }
                else
                {
                    cChars[ccLine].Add(new(coloredString[readIDx], fgColorStack.Last.Data, bgColorStack.Last.Data));
                }
                readIDx++;
            }

            Lines = new ColoredChar[cChars.Length][];
            int rows = Rows;
            for (int row = 0; row < rows; row++)
            {
                Lines[row] = new ColoredChar[cChars[row].Length];
                int cols = Cols(row);
                for (int col = 0; col < cols; col++)
                {
                    Lines[row][col] = cChars[row][col];
                }
            }
        }

        /// <summary>
        /// Translate the escape codes to ConsoleColor enum values
        /// </summary>
        /// <param name="str">A 2 character long string</param>
        /// <returns>The color encoded by <paramref name="str"/>. If cannot be translated White is returned</returns>
        private ConsoleColor ParseEscapeCode(string str)
        {
            if (int.TryParse(str, out int colorCode) && colorCode <= 15 && colorCode >= 0)
            {
                return (ConsoleColor)colorCode;
            }
            return ConsoleColor.White;
        }
    }
}
