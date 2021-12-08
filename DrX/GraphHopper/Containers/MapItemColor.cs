using DrX.Utils;
using System;

namespace DrX.GraphHopper.Containers
{
    /// <summary>
    /// Represents a colored item on the <see cref="Map"/>
    /// </summary>
    public class MapItemColor
    {
        public Coordinate Coordinate { get; set; } = Coordinate.Zero;
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

        public MapItemColor() { }

        public MapItemColor(Coordinate coord, ConsoleColor fColor, ConsoleColor bColor)
        {
            Coordinate = new(coord);
            ForegroundColor = fColor;
            BackgroundColor = bColor;
        }
    }
}
