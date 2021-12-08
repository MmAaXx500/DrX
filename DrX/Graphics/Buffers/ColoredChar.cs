using System;

namespace DrX.Graphics.Buffers
{
    /// <summary>
    /// Represents a character which have a background and a foreground color.
    /// </summary>
    public class ColoredChar
    {
        public char Char { get; set; } = ' ';
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

        public ColoredChar() { }

        public ColoredChar(ColoredChar cCahr)
        {
            Char = cCahr.Char;
            ForegroundColor = cCahr.ForegroundColor;
            BackgroundColor = cCahr.BackgroundColor;
        }

        public ColoredChar(char c)
        {
            Char = c;
        }

        public ColoredChar(char c, ConsoleColor fColor, ConsoleColor bColor)
        {
            Char = c;
            ForegroundColor = fColor;
            BackgroundColor = bColor;
        }

        public override bool Equals(object obj)
        {
            if (obj is ColoredChar cc)
                return cc.Char == Char
                       && cc.BackgroundColor == BackgroundColor
                       && cc.ForegroundColor == ForegroundColor;
            return false;
        }

        public override int GetHashCode()
        {
            return Char ^ (int)ForegroundColor ^ (int)BackgroundColor;
        }
    }
}
