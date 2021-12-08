using DrX.Utils.Generic;

namespace DrX.GraphHopper
{
    /// <summary>
    /// Represent a vertex in the Graph
    /// </summary>
    public class Vertex
    {
        public LinkedList<char> ConnectedVertexIDs { get; set; } = new LinkedList<char>();
        public char ID { get; set; }

        public Vertex() { }

        public Vertex(Vertex vertex)
        {
            ConnectedVertexIDs = new LinkedList<char>(vertex.ConnectedVertexIDs);
            ID = vertex.ID;
        }

        public override bool Equals(object obj)
        {
            if(obj is Vertex other)
                return other.ID == ID;
            return false;
        }

        public override int GetHashCode()
        {
            return ID;
        }
    }
}
