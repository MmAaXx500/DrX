namespace DrX.Utils
{
    /// <summary>
    /// Represents two dimensions by rows and columns.
    /// </summary>
    public class Dimension
    {
        public readonly static Dimension Zero = new(0, 0);
        public int Rows { get; set; }
        public int Cols { get; set; }

        public Dimension() { }

        public Dimension(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
        }

        public override bool Equals(object obj)
        {
            if (obj is Dimension other)
                return other.Rows == Rows && other.Cols == Cols;
            return false;
        }

        public override int GetHashCode()
        {
            return Rows ^ Cols;
        }
    }
}
