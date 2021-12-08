
namespace DrX.Utils
{
    /// <summary>
    /// Represents two dimensions by X and Y values.<br/>
    /// In matrices Y is the row and X is the column. 
    /// </summary>
    public class Coordinate
    {
        public static Coordinate Zero { get; } = new(0, 0);
        public int Y { get; set; }
        public int X { get; set; }

        public Coordinate() { }

        public Coordinate(int y, int x)
        {
            Y = y;
            X = x;
        }

        public Coordinate(Coordinate coord)
        {
            Y = coord.Y;
            X = coord.X;
        }

        public static Coordinate operator +(Coordinate a, Coordinate b)
        {
            return new(a.Y + b.Y, a.X + b.X);
        }

        public override bool Equals(object obj)
        {
            if (obj is Coordinate other)
                return other.Y == Y && other.X == X;
            return false;
        }

        public override int GetHashCode()
        {
            return Y ^ X;
        }
    }
}
