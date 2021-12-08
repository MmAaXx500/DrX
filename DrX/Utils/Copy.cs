using DrX.GraphHopper;
using DrX.Utils.Generic;

namespace DrX.Utils
{
    static class Copy
    {
        public static LinkedList<Vertex> VetrexList(LinkedList<Vertex> list)
        {
            LinkedList<Vertex> newList = new();
            for (LinkedListNode<Vertex> vrtx = list.First; vrtx != null; vrtx = vrtx.Next)
            {
                Vertex newVertex = new(vrtx.Data);
                newList.Add(newVertex);
            }
            return newList;
        }
    }
}
