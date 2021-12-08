using DrX.GraphHopper.Containers;
using DrX.Graphics.Buffers;
using DrX.Utils;

namespace DrX.GraphHopper
{
    /// <summary>
    /// Static class of graphical elements used by the graphics engine
    /// </summary>
    static class Sprite
    {
        /// <summary>
        /// Maps <c>PathSegment</c> enum to character representations
        /// </summary>
        /// <param name="pathSegment"></param>
        /// <returns>Char matrix representation of the path segment</returns>
        public static char[,] PathSegmentToSprite(PathSegment pathSegment)
        {
            return pathSegment switch
            {
                PathSegment.Empty => empty,
                PathSegment.Horizontal => horizontal,
                PathSegment.Vertical => vertical,
                PathSegment.BottomLeftToTopRight => bottomLeftToTopRight,
                PathSegment.TopLeftToBottomRight => topLeftToBottomRight,
                PathSegment.BottomLeftToRight => bottomLeftToRight,
                PathSegment.TopLeftToRight => topLeftToRight,
                PathSegment.LeftToTopRight => leftToTopRight,
                PathSegment.LeftToBottomRight => leftToBottomRight,
                PathSegment.BottomLeftToTop => bottomLeftToTop,
                PathSegment.TopLeftToBottom => topLeftToBottom,
                PathSegment.TopToBottomRight => topToBottomRight,
                PathSegment.BottomToTopRight => bottomToTopRight,
                PathSegment.LeftToTop => leftToTop,
                PathSegment.LeftToBottom => leftToBottom,
                PathSegment.TopToRight => topToRight,
                PathSegment.BottomToRight => bottomToRight,
                _ => empty,
            };
        }

        /// <summary>
        /// Generates a char matrix with the provided character embedded in the center
        /// </summary>
        /// <param name="ID">Character to be placed in the center</param>
        /// <returns>A char matrix with the provided character embedded in the center</returns>
        public static char[,] VertexToSprite(char ID)
        {
            char[,] vrtx = new char[3, 5];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    vrtx[i, j] = vertex[i, j];
                }
            }
            vrtx[1, 2] = ID;
            return vrtx;
        }

        /// <summary>
        /// Placeholder template when we can't generate the map
        /// </summary>
        /// <returns><c>BlockBuffer</c> representation of the template</returns>
        public static BlockBuffer GetPlaceholder()
        {
            Dimension d = new(placeholder.GetLength(0), placeholder.GetLength(1));
            BlockBuffer bf = new(new(1, 1), d);
            for (int i = 0; i < bf.Blocks[0, 0].Data.GetLength(0); i++)
            {
                for (int j = 0; j < bf.Blocks[0, 0].Data.GetLength(1); j++)
                {
                    bf.Blocks[0, 0].Data[i, j] = placeholder[i, j];
                }
            }
            return bf;
        }

        private static readonly char[,] empty =
        {
            {' ',' ',' ',' ',' ' },
            {' ',' ',' ',' ',' ' },
            {' ',' ',' ',' ',' ' },
        };

        private static readonly char[,] horizontal =
        {
            {' ',' ',' ',' ',' ' },
            {'─','─','─','─','─' },
            {' ',' ',' ',' ',' ' },
        };

        private static readonly char[,] vertical =
        {
            {' ',' ','│',' ',' ' },
            {' ',' ','│',' ',' ' },
            {' ',' ','│',' ',' ' },
        };

        private static readonly char[,] bottomLeftToTopRight =
        {
            {' ',' ',' ',' ','╱' },
            {' ',' ','╱',' ',' ' },
            {'╱',' ',' ',' ',' ' },
        };

        private static readonly char[,] topLeftToBottomRight =
        {
            {'╲',' ',' ',' ',' ' },
            {' ',' ','╲',' ',' ' },
            {' ',' ',' ',' ','╲' },
        };

        private static readonly char[,] bottomLeftToRight =
        {
            {' ',' ',' ',' ',' ' },
            {' ','─','─','─','─' },
            {'╱',' ',' ',' ',' ' },
        };

        private static readonly char[,] topLeftToRight =
        {
            {'╲',' ',' ',' ',' ' },
            {' ','─','─','─','─' },
            {' ',' ',' ',' ',' ' },
        };

        private static readonly char[,] leftToTopRight =
        {
            {' ',' ',' ',' ','╱' },
            {'─','─','─','─',' ' },
            {' ',' ',' ',' ',' ' },
        };

        private static readonly char[,] leftToBottomRight =
        {
            {' ',' ',' ',' ',' ' },
            {'─','─','─','─',' ' },
            {' ',' ',' ',' ','╲' },
        };

        private static readonly char[,] bottomLeftToTop =
        {
            {' ',' ','│',' ',' ' },
            {' ','╱',' ',' ',' ' },
            {'╱',' ',' ',' ',' ' },
        };

        private static readonly char[,] topLeftToBottom =
        {
            {'╲',' ',' ',' ',' ' },
            {' ','╲',' ',' ',' ' },
            {' ',' ','│',' ',' ' },
        };

        private static readonly char[,] topToBottomRight =
        {
            {' ',' ','│',' ',' ' },
            {' ',' ',' ','╲',' ' },
            {' ',' ',' ',' ','╲' },
        };

        private static readonly char[,] bottomToTopRight =
        {
            {' ',' ',' ',' ','╱' },
            {' ',' ',' ','╱',' ' },
            {' ',' ','│',' ',' ' },
        };

        private static readonly char[,] leftToTop =
        {
            {' ',' ','│',' ',' ' },
            {'─','─','┘',' ',' ' },
            {' ',' ',' ',' ',' ' },
        };

        private static readonly char[,] leftToBottom =
        {
            {' ',' ',' ',' ',' ' },
            {'─','─','┐',' ',' ' },
            {' ',' ','│',' ',' ' },
        };

        private static readonly char[,] topToRight =
        {
            {' ',' ','│',' ',' ' },
            {' ',' ','└','─','─' },
            {' ',' ',' ',' ',' ' },
        };

        private static readonly char[,] bottomToRight =
        {
            {' ',' ',' ',' ',' ' },
            {' ',' ','┌','─','─' },
            {' ',' ','│',' ',' ' },
        };

        private static readonly char[,] vertex =
        {
            {'┏','━','━','━','┓' },
            {'┃',' ',' ',' ','┃' },
            {'┗','━','━','━','┛' },
        };
		
        // The placeholder was generated by this site: https://patorjk.com/software/taag/#p=display&h=1&v=1&f=Big&t=DrX%0A
        private static readonly char[,] placeholder =
        {
            { ' ', '_', '_', '_', '_', '_', ' ', ' ', ' ', ' ', ' ', ' ', '_', '_', ' ', ' ', ' ', '_', '_' },
            { '|', ' ', ' ', '_', '_', ' ', '\\', ' ', ' ', ' ', ' ', ' ', '\\', ' ', '\\', ' ', '/', ' ', '/' },
            { '|', ' ', '|', ' ', ' ', '|', ' ', '|', ' ', '_', ' ', '_', '_', '\\', ' ', 'V', ' ', '/', ' ' },
            { '|', ' ', '|', ' ', ' ', '|', ' ', '|', '|', ' ', '\'','_', '_', '|', '>', ' ', '<', ' ', ' ' },
            { '|', ' ', '|', '_', '_', '|', ' ', '|', '|', ' ', '|', ' ', ' ', '/', ' ', '.', ' ', '\\', ' ' },
            { '|', '_', '_', '_', '_', '_', '/', ' ', '|', '_', '|', ' ', '/', '_', '/', ' ', '\\', '_', '\\' }
        };
    }
}
