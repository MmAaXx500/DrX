using DrX.Utils;
using DrX.Utils.Generic;

namespace DrX.GraphHopper.Containers
{
    /// <summary>
    /// Represents the game map
    /// </summary>
    public class Map
    {
        private readonly List<List<MapItem>> mapItems = new();

        public MapItem this[int i, int j]
        {
            get { return mapItems[i][j]; }
            set { mapItems[i][j] = value; }
        }
        public MapItem this[Coordinate coord]
        {
            get { return mapItems[coord.Y][coord.X]; }
            set { mapItems[coord.Y][coord.X] = value; }
        }

        public int Rows => mapItems.Length;
        public int Cols
        {
            get
            {
                if (Rows == 0)
                    return 0;
                return mapItems[0].Length;
            }
        }

        public Map() { }

        public Map(Map map)
        {
            for (int row = 0; row < map.Rows; row++)
            {
                AddRowBottom();
                for (int col = 0; col < map.Cols; col++)
                {
                    if (row == 0)
                        AddColRight();

                    mapItems[row][col] = new MapItem(map[row, col]);
                }
            }
        }

        /// <summary>
        /// Add a row at the bottom of the map
        /// </summary>
        public void AddRowBottom()
        {
            List<MapItem> newRow = new();
            for (int i = 0; i < Cols; i++)
            {
                newRow.Add(new MapItem());
            }
            mapItems.Add(newRow);
        }

        /// <summary>
        /// Add the specified amount of rows at the bottom of the map
        /// </summary>
        public void AddRowBottom(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                AddRowBottom();
            }
        }

        /// <summary>
        /// Add a row at the top of the map
        /// </summary>
        public void AddRowTop()
        {
            List<MapItem> newRow = new();
            for (int i = 0; i < Cols; i++)
            {
                newRow.Add(new MapItem());
            }
            mapItems.AddAt(newRow, 0);
        }

        /// <summary>
        /// Add the specified amount of rows at the top of the map
        /// </summary>
        public void AddRowTop(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                AddRowTop();
            }
        }

        /// <summary>
        /// Add a column to the right. Beware: You must add a row first
        /// </summary>
        public void AddColRight()
        {
            for (int i = 0; i < Rows; i++)
            {
                mapItems[i].Add(new MapItem());
            }
        }

        /// <summary>
        /// Add the specified amount of columns to the right. Beware: You must add a row first
        /// </summary>
        public void AddColRight(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                AddColRight();
            }
        }

        /// <summary>
        /// Add the specified amount of columns to the left. Beware: You must add a row first
        /// </summary>
        public void AddColLeft()
        {
            for (int i = 0; i < Rows; i++)
            {
                mapItems[i].AddAt(new MapItem(), 0);
            }
        }

        /// <summary>
        /// Add a column to the left. Beware: You must add a row first
        /// </summary>
        public void AddColLeft(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                AddColLeft();
            }
        }

        /// <summary>
        /// Add a border to all sides of the map
        /// </summary>
        /// <param name="width">Width of the border</param>
        public void AddBorder(int width)
        {
            for (int i = 0; i < width; i++)
            {
                AddRowTop();
                AddRowBottom();
                AddColLeft();
                AddColRight();
            }
        }

        /// <summary>
        /// Checks if the map has the specified coordinates
        /// </summary>
        /// <param name="coord">The coordinate to check</param>
        /// <returns>True if <paramref name="coord"/> is on the map. False otherwise</returns>
        public bool HasCoordinate(Coordinate coord)
        {
            return coord.Y >= 0 && coord.Y < Rows
                && coord.X >= 0 && coord.X < Cols;
        }

        /// <summary>
        /// Get coordinate of an <c>MapItem</c>
        /// </summary>
        /// <param name="mapItem"><c>MapItem</c> to be searched for</param>
        /// <returns>Coordinates of the <paramref name="mapItem"/>. If not found (-1,-1) is returned</returns>
        public Coordinate CoordOf(MapItem mapItem)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    if (mapItems[row][col].Equals(mapItem))
                        return new Coordinate(row, col);
                }
            }
            return new Coordinate(-1, -1);
        }

        /// <summary>
        /// Get coordinate of <c>MapItem</c> which has the <paramref name="ID"/>
        /// </summary>
        /// <param name="ID">ID to be searched for</param>
        /// <returns>Coordinates of the <paramref name="ID"/>. If not found (-1,-1) is returned</returns>
        public Coordinate CoordOf(char ID)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    if (mapItems[row][col].ID == ID)
                        return new Coordinate(row, col);
                }
            }
            return new Coordinate(-1, -1);
        }

        /// <summary>
        /// Get the adjacent <c>MapItem</c>s of <paramref name="mapItem"/>
        /// </summary>
        /// <param name="mapItem"><c>MapItem</c> in the "center"</param>
        /// <returns>List of adjacent <c>MapItem</c>s</returns>
        public LinkedList<MapItem> AdjacentItemsOf(MapItem mapItem)
        {
            Coordinate coord = CoordOf(mapItem);
            if (coord.Y == -1)
                return new LinkedList<MapItem>();
            return AdjacentItemsOf(coord);
        }

        /// <summary>
        /// Get the adjacent <c>MapItem</c>s <paramref name="coordinate"/>
        /// </summary>
        /// <param name="coordinate">Coordinate of the <c>MapItem</c> in the "center"</param>
        /// <returns>List of adjacent <c>MapItem</c>s</returns>
        public LinkedList<MapItem> AdjacentItemsOf(Coordinate coordinate)
        {
            LinkedList<Coordinate> adjCoords = AdjacentCoordinatesOf(coordinate);
            LinkedList<MapItem> adjs = new();
            for (LinkedListNode<Coordinate> coord = adjCoords.First; coord != null; coord = coord.Next)
            {
                Coordinate c = coord.Data;
                adjs.Add(new(mapItems[c.Y][c.X]));
            }
            return adjs;
        }

        /// <summary>
        /// Get all <c>MapItem</c> can be found in the map
        /// </summary>
        /// <returns>List of <c>MapItem</c>s in the map. The list can have zero length</returns>
        public LinkedList<MapItem> GetListOfVerticesByRef()
        {
            LinkedList<MapItem> vertices = new();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    if (mapItems[row][col].Status == Status.Vertex)
                        vertices.Add(mapItems[row][col]);
                }
            }
            return vertices;
        }

        /// <summary>
        /// Get all coordinates which contains an item with status of vertex
        /// </summary>
        /// <returns>List of coordinates in the map. The list can have zero length</returns>
        public LinkedList<Coordinate> GetListOfVertexCoords()
        {
            LinkedList<Coordinate> coords = new();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    if (mapItems[row][col].Status == Status.Vertex)
                        coords.Add(new(row, col));
                }
            }
            return coords;
        }

        /// <summary>
        /// Get the adjacent coordinates of <paramref name="mapItem"/>
        /// </summary>
        /// <param name="mapItem"><c>MapItem</c> in the "center"</param>
        /// <returns>List of adjacent coordinates</returns>
        public LinkedList<Coordinate> AdjacentCoordinatesOf(MapItem mapItem)
        {
            Coordinate coord = CoordOf(mapItem);
            if (coord.Y == -1)
                return new LinkedList<Coordinate>();
            return AdjacentCoordinatesOf(coord);
        }

        /// <summary>
        /// Get the adjacent coordinates of <paramref name="coordinate"/>
        /// </summary>
        /// <param name="coordinate">Coordinate in the "center"</param>
        /// <returns>List of adjacent coordinates</returns>
        public LinkedList<Coordinate> AdjacentCoordinatesOf(Coordinate coordinate)
        {
            LinkedList<Coordinate> acs = new();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!(i == 0 && j == 0))
                    {
                        int rowIdx = coordinate.Y + i;
                        int colIdx = coordinate.X + j;
                        if (rowIdx < Rows
                            && colIdx < Cols
                            && colIdx >= 0
                            && rowIdx >= 0)
                        {
                            acs.Add(new(rowIdx, colIdx));
                        }
                    }
                }
            }
            return acs;
        }
    }
}
