using DrX.Utils.Generic;

namespace DrX.GraphHopper.Containers
{
    /// <summary>
    /// Represents an item on the <see cref="Map"/>
    /// </summary>
    public class MapItem
    {
        public Status Status { get; set; }
        public PathSegment PathSegment { get; set; }
        public char ID { get; set; }
        public LinkedList<char> Connections { get; set; } = new();
        public LinkedList<char> MissingConnections { get; set; } = new();

        public int Cost
        {
            get
            {
                return Status switch
                {
                    Status.Empty => 1,
                    Status.Path => 10000,
                    Status.Vertex => 10000,
                    _ => 10000,
                };
            }
        }

        public MapItem()
        {
            Status = Status.Empty;
            PathSegment = PathSegment.Empty;
        }

        public MapItem(char ID, LinkedList<char> allConns, LinkedList<char> requiredConns)
        {
            this.ID = ID;
            Connections = new(allConns);
            MissingConnections = new(requiredConns);

            if (ID == default(char))
                Status = Status.Empty;
            else
                Status = Status.Vertex;

            PathSegment = PathSegment.Empty;
        }

        public MapItem(PathSegment pathSegment)
        {
            PathSegment = pathSegment;

            if (pathSegment == PathSegment.Empty)
                Status = Status.Empty;
            else
                Status = Status.Path;
        }

        public MapItem(MapItem mapItem)
        {
            Status = mapItem.Status;
            PathSegment = mapItem.PathSegment;
            ID = mapItem.ID;
            //fulfilledConns = new LinkedList<char>(mapItem.fulfilledConns);
        }

        public override bool Equals(object obj)
        {
            if (obj is MapItem other)
                return other.ID == ID && other.PathSegment == PathSegment && other.Status == Status;
            return false;
        }

        public override int GetHashCode()
        {
            return (int)Status ^ (int)PathSegment ^ ID;
        }
    }
}
