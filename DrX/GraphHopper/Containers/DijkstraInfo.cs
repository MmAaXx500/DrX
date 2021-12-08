using DrX.Utils;

namespace DrX.GraphHopper.Containers
{
    /// <summary>
    /// Represents an entry in the Dijkstra's path-finding algorithm
    /// </summary>
    public class DijkstraInfo
    {
        public Coordinate Coord { get; set; }
        public int Cost { get; set; }
        public Coordinate PreviousCoord { get; set; }

        /// <summary>
        /// Creates an instance with <c>Cost</c> set to int.MaxValue
        /// </summary>
        /// <param name="coord">Position on the map</param>
        public DijkstraInfo(Coordinate coord)
        {
            Coord = coord;
            Cost = int.MaxValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is DijkstraInfo di)
                return di.Coord.Equals(Coord);
            return false;
        }

        public override int GetHashCode()
        {
            return Coord.GetHashCode();
        }
    }
}
