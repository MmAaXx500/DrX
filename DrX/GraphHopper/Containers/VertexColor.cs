using System;

namespace DrX.GraphHopper.Containers
{
    /// <summary>
    /// Represents a colored Vertex
    /// </summary>
    public class VertexColor
    {
        public char ID { get; set; }
        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }

        public VertexColor(char ID, ConsoleColor fColor = ConsoleColor.White, ConsoleColor bColor = ConsoleColor.Black)
        {
            this.ID = ID;
            ForegroundColor = fColor;
            BackgroundColor = bColor;
        }
    }
}
