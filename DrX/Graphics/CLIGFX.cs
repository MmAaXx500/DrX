using DrX.Graphics.Buffers;
using DrX.Utils;
using System;

namespace DrX.Graphics
{
    /// <summary>
    /// The graphics engine.<br/>
    /// This is a singleton class do not instantiate!
    /// Get the only instance with <see cref="GetInstance"/><br/>
    /// <br/>
    /// Supports dynamic window resizing.<br/>
    /// Separates the screen to an upper part and a lower part.<br/>
    /// The upper part contains the graph which supports panning with the arrow keys any time.<br/>
    /// The lower part contains the line buffer which supports text reflow based on the window size.
    /// </summary>
    public class CLIGFX
    {
        private static CLIGFX instance;

        private BlockBuffer graphBuffer;
        private LineBuffer lineBuffer;
        private ScreenBuffer lastScreen;
        private bool isBuffersChanged;
        private Dimension lastWindowSize;
        private int graphOffsetHorizontal;
        private int graphOffsetVertical;

        /// <summary>
        /// Get the instance of CLIGFX
        /// </summary>
        /// <returns>The instance</returns>
        public static CLIGFX GetInstance()
        {
            if (instance == null)
                instance = new CLIGFX();

            return instance;
        }

        /// <summary>
        /// Set the graph buffer. The graph buffer is displayed in the upper part of the window.
        /// </summary>
        /// <param name="graphBuffer">A <see cref="BlockBuffer"/>r instance</param>
        public void SetGraphBuffer(BlockBuffer graphBuffer)
        {
            this.graphBuffer = new(graphBuffer);
            isBuffersChanged = true;
        }

        /// <summary>
        /// Set the line buffer. The line buffer is displayed on the lower part of the window.
        /// </summary>
        /// <param name="lineBuffer">A <see cref="LineBuffer"/> instance</param>
        public void SetLineBuffer(LineBuffer lineBuffer)
        {
            this.lineBuffer = new(lineBuffer);
            isBuffersChanged = true;
        }

        /// <summary>
        /// Set the line buffer from a string. The line buffer is displayed on the lower part of the window.
        /// </summary>
        /// <param name="lineBuffer">A string formatted with the <see cref="LineBuffer"/> supported formats</param>
        public void SetLineBuffer(string lineBuffer)
        {
            this.lineBuffer = new(lineBuffer);
            isBuffersChanged = true;
        }

        /// <summary>
        /// Redraws the screen
        /// </summary>
        public void RefreshScreen()
        {
            Dimension windowSize = GetUsableWindowSize();
            bool isWindowSizeChanged = !windowSize.Equals(lastWindowSize);
            if (isBuffersChanged || isWindowSizeChanged)
            {
                if (isWindowSizeChanged)
                {
                    lastWindowSize = windowSize;

                    Console.Clear();

                    graphOffsetHorizontal = 0;
                    graphOffsetVertical = 0;
                }

                isBuffersChanged = false;
                int graphSpaceHeight = windowSize.Rows - lineBuffer.RequiredRowsAtDimension(windowSize);

                Dimension graphWindow = new(graphSpaceHeight, windowSize.Cols);
                ScreenBuffer screenBuffer = new(windowSize);

                AddGraphToScreenBuffer(screenBuffer, graphWindow);

                AddLineBufferToSB(screenBuffer, graphSpaceHeight);

                if (screenBuffer.Equals(lastScreen) == false)
                {

                    //UpdateScreen(screenBuffer, windowSize);
                    FlushScreenBuffer(screenBuffer, windowSize);
                    lastScreen = screenBuffer;
                }
            }
        }

        /// <summary></summary>
        /// <returns>True if any key available for processing</returns>
        public bool IsKeyAvailable()
        {
            return Console.KeyAvailable;
        }

        /// <summary>
        /// Reads user input. Handles backspace and captures arrow keys to move the graph but passes to the caller too. 
        /// </summary>
        /// <returns>The <see cref="ConsoleKeyInfo"/> of the pressed key</returns>
        public ConsoleKeyInfo ReadUserInput()
        {
            ConsoleKeyInfo cki = Console.ReadKey();
            Dimension ws = GetWindowSize();

            switch (cki.Key)
            {
                case ConsoleKey.Backspace:
                    Console.Write(" \b");
                    break;

                case ConsoleKey.UpArrow:
                    graphOffsetVertical -= ws.Rows / (4 * graphBuffer.BlockSize.Rows);
                    isBuffersChanged = true;
                    break;

                case ConsoleKey.DownArrow:
                    graphOffsetVertical += ws.Rows / (4 * graphBuffer.BlockSize.Rows);
                    isBuffersChanged = true;
                    break;

                case ConsoleKey.LeftArrow:
                    graphOffsetHorizontal -= ws.Cols / (4 * graphBuffer.BlockSize.Cols);
                    isBuffersChanged = true;
                    break;

                case ConsoleKey.RightArrow:
                    graphOffsetHorizontal += ws.Cols / (4 * graphBuffer.BlockSize.Cols);
                    isBuffersChanged = true;
                    break;
            }

            return cki;
        }

        /// <summary>
        /// Clears the last lie of the window.
        /// </summary>
        public void ClearInputLine()
        {
            SetCaretToInput();
            Dimension ws = GetWindowSize();
            string clear = "".PadRight(ws.Cols);
            Console.Write(clear);
            SetCaretToInput();
        }

        /// <summary>
        /// Private constructor to prevent instantiation
        /// </summary>
        private CLIGFX()
        {
            graphBuffer = new(Dimension.Zero, Dimension.Zero);
            lineBuffer = new(0);
            isBuffersChanged = true;
            lastScreen = new(Dimension.Zero);

            InitConsole();

            lastWindowSize = GetUsableWindowSize();
        }

        /// <summary>
        /// Initialize the console for the engine.
        /// </summary>
        private void InitConsole()
        {
            Console.InputEncoding = System.Text.Encoding.Unicode;
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
        }

        /// <summary></summary>
        /// <returns>The usable windows size. This size do not contain the input line.</returns>
        private Dimension GetUsableWindowSize()
        {
            int offsetFromBottom = 1; // keep a line clean for user input
            return new(Console.WindowHeight - offsetFromBottom, Console.WindowWidth - 1);
        }

        /// <summary></summary>
        /// <returns>The full window size includong the input line.</returns>
        private Dimension GetWindowSize()
        {
            return new(Console.WindowHeight, Console.WindowWidth);
        }

        /// <summary>
        /// Add a part of the graph buffer to the specified <paramref name="screenBuffer"/> based on the <paramref name="graphWindow"/> size.
        /// </summary>
        /// <param name="screenBuffer">The screen buffer</param>
        /// <param name="graphWindow">The available space for the graph</param>
        private void AddGraphToScreenBuffer(ScreenBuffer screenBuffer, Dimension graphWindow)
        {
            if (graphBuffer.BlockSize.Rows != 0 && graphBuffer.BlockSize.Cols != 0)
            {
                int startRowInGB = 0;
                int startColInGB = 0;
                CalcGraphPosition(graphWindow, ref startRowInGB, ref startColInGB);

                int graphRowIdx = startRowInGB;
                for (int screenBuffRow = 0; screenBuffRow < graphWindow.Rows; screenBuffRow += graphBuffer.BlockSize.Rows)
                {
                    int graphColIdx = startColInGB;
                    for (int screenBuffCol = 0; screenBuffCol < graphWindow.Cols; screenBuffCol += graphBuffer.BlockSize.Cols)
                    {
                        if (IsBetween(graphRowIdx, 0, graphBuffer.BufferSize.Rows)
                            && IsBetween(graphColIdx, 0, graphBuffer.BufferSize.Cols))
                        {
                            Block gbBlock = graphBuffer.Blocks[graphRowIdx, graphColIdx];
                            AddGraphBlockToSB(screenBuffer, gbBlock, graphWindow, screenBuffRow, screenBuffCol);
                        }
                        graphColIdx++;
                    }
                    graphRowIdx++;
                }
            }
        }

        /// <summary>
        /// Add a <paramref name="graphBufferBlock"/> to the specified <paramref name="screenBuffer"/>.
        /// </summary>
        /// <param name="screenBuffer">The screen buffer</param>
        /// <param name="graphBufferBlock">The block to be added</param>
        /// <param name="graphWindow">The available space for the graph</param>
        /// <param name="screenBuffRowIdx">Offset the <paramref name="graphBufferBlock"/> in the <paramref name="screenBuffer"/></param>
        /// <param name="screenBuffColIdx">Offset the <paramref name="graphBufferBlock"/> in the <paramref name="screenBuffer"/></param>
        private void AddGraphBlockToSB(ScreenBuffer screenBuffer, Block graphBufferBlock, Dimension graphWindow, int screenBuffRowIdx, int screenBuffColIdx)
        {
            for (int graphBlockRow = 0; graphBlockRow < graphBuffer.BlockSize.Rows; graphBlockRow++)
            {
                int row = graphBlockRow + screenBuffRowIdx;
                for (int graphBlockCol = 0; graphBlockCol < graphBuffer.BlockSize.Cols; graphBlockCol++)
                {
                    int col = graphBlockCol + screenBuffColIdx;
                    if (row < graphWindow.Rows
                        && col < graphWindow.Cols)
                    {
                        char graphChar = graphBufferBlock[graphBlockRow, graphBlockCol];
                        ConsoleColor fColor = graphBufferBlock.ForegroundColor;
                        ConsoleColor bColor = graphBufferBlock.BackgroundColor;

                        screenBuffer[row, col] = new(graphChar, fColor, bColor);
                    }
                }
            }
        }

        /// <summary>
        /// Add line buffer to the <paramref name="screenBuffer"/>
        /// </summary>
        /// <param name="screenBuffer">The screen buffer</param>
        /// <param name="sbRowStartIdx">Offset the starting row in the <paramref name="screenBuffer"/></param>
        private void AddLineBufferToSB(ScreenBuffer screenBuffer, int sbRowStartIdx)
        {
            int sbRow = sbRowStartIdx;
            int sbCol = 0;
            int lbRow = 0;
            int lbCol = 0;

            while (IsBetween(sbRow, 0, screenBuffer.Rows) && IsBetween(lbRow, 0, lineBuffer.Rows))
            {
                if (lineBuffer.Cols(lbRow) > 0)
                {
                    ColoredChar cc = lineBuffer[lbRow, lbCol];
                    screenBuffer[sbRow, sbCol] = new(cc.Char, cc.ForegroundColor, cc.BackgroundColor);
                }

                sbCol++;
                lbCol++;

                bool sbLF = false;
                if (sbCol >= screenBuffer.Cols)
                {
                    sbCol = 0;
                    sbRow++;
                    sbLF = true;
                }

                if (lbCol >= lineBuffer.Cols(lbRow))
                {
                    lbCol = 0;
                    lbRow++;
                    if (!sbLF)
                    {
                        sbCol = 0;
                        sbRow++;
                    }
                }
            }
        }

        /// <summary>
        /// Writes to contents of the <paramref name="screenBuffer"/> to the console.
        /// </summary>
        /// <param name="screenBuffer"></param>
        /// <param name="windowDimension"></param>
        private void FlushScreenBuffer(ScreenBuffer screenBuffer, Dimension windowDimension)
        {
            (int orgCol, int orgRow) = Console.GetCursorPosition();
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);
            Console.Write(screenBuffer.GetAsString());

            FlushColors(screenBuffer, windowDimension);

            if (orgCol != 0 && orgRow == GetWindowSize().Rows - 1) // there is some input, keep it
            {
                Console.SetCursorPosition(orgCol, orgRow);
                Console.CursorVisible = true;
            }
            else
                ClearInputLine();
        }

        /// <summary>
        /// Writes colors from the <paramref name="screenBuffer"/> to the console
        /// </summary>
        /// <param name="screenBuffer"></param>
        /// <param name="windowDimension"></param>
        private void FlushColors(ScreenBuffer screenBuffer, Dimension windowDimension)
        {
            int consoleCol = -1;
            int consoleRow = -1;
            for (int row = 0; row < windowDimension.Rows; row++)
            {
                for (int col = 0; col < windowDimension.Cols; col++)
                {
                    if (screenBuffer[row, col].ForegroundColor != ConsoleColor.White
                        || screenBuffer[row, col].BackgroundColor != ConsoleColor.Black)
                    {
                        Dimension windowSizeCheck = GetUsableWindowSize();
                        if (windowSizeCheck.Equals(windowDimension) == false)
                            return;

                        if (consoleCol != col || consoleRow != row)
                        {
                            Console.SetCursorPosition(col, row);
                            consoleRow = row;
                        }
                        consoleCol = col + 1; // C.Write() moves cursor right by one

                        Console.ForegroundColor = screenBuffer[row, col].ForegroundColor;
                        Console.BackgroundColor = screenBuffer[row, col].BackgroundColor;

                        Console.Write(screenBuffer[row, col].Char);

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the graph position based on the <paramref name="graphWindow"/> and the graph size.
        /// </summary>
        /// <param name="graphWindow">Space for the graph</param>
        /// <param name="graphRowStartIdx">Returns the row index of the first block to be written out</param>
        /// <param name="graphColStartIdx">Returns the column index of the first block to be written out</param>
        private void CalcGraphPosition(Dimension graphWindow, ref int graphRowStartIdx, ref int graphColStartIdx)
        {
            Dimension graphDimension = graphBuffer.BufferSize;
            graphRowStartIdx = graphDimension.Rows / 2 - graphWindow.Rows / (2 * graphBuffer.BlockSize.Rows) + graphOffsetVertical;
            graphColStartIdx = graphDimension.Cols / 2 - graphWindow.Cols / (2 * graphBuffer.BlockSize.Cols) + graphOffsetHorizontal;
        }

        /// <summary></summary>
        /// <param name="value"></param>
        /// <param name="lower">Lower limit. Included.</param>
        /// <param name="upper">Upper limit. Excluded.</param>
        /// <returns>True if the <paramref name="value"/> is between the <paramref name="lower"/> and <paramref name="upper"/> limits.</returns>
        private bool IsBetween(int value, int lower, int upper)
        {
            return value >= lower && value < upper;
        }

        /// <summary>
        /// Positions the cursor to the input line and sets visible.
        /// </summary>
        private void SetCaretToInput()
        {
            Dimension ws = GetWindowSize();
            Console.SetCursorPosition(0, ws.Rows - 1);
            Console.CursorVisible = true;
        }
    }
}
