using DrX.GraphHopper.Containers;
using DrX.Graphics.Buffers;
using DrX.Utils;
using DrX.Utils.Generic;
using System;

namespace DrX.GraphHopper
{
    public class Graph
    {
        private readonly LinkedList<Vertex> vertices = new();
        private readonly Map map = new();
        private BlockBuffer bf;
        private bool generateMap;

        /// <summary></summary>
        /// <param name="mapLines">Lines of map.txt</param>
        /// <param name="generateMap">Try to generate game map</param>
        public Graph(string[] mapLines, bool generateMap = true)
        {
            this.generateMap = generateMap;
            ParseMap(mapLines);
            if (generateMap)
                GenerateGraphLayout();
            else
                FillPlaceholder();
        }

        /// <summary>
        /// Generates the contents of the BlockBuffer with optional coloring
        /// </summary>
        /// <returns>The generated BlockBuffer</returns>
        public BlockBuffer GenerateBlockBuffer()
        {
            return GenerateBlockBuffer(new());
        }

        /// <summary>
        /// Generates the contents of the BlockBuffer with optional coloring
        /// </summary>
        /// <param name="vrtxColor">List of VertexColors</param>
        /// <returns>The generated BlockBuffer</returns>
        public BlockBuffer GenerateBlockBuffer(List<VertexColor> vrtxColor)
        {
            if (generateMap)
            {
                MapItemColor[] mcs = ColoredVrtxToMapColor(vrtxColor);
                FillBlockBuffer(mcs);
            }
            return bf;
        }

        /// <summary>
        /// Get the vertices connected to <paramref name="vertex"/>
        /// </summary>
        /// <returns>List of vertices connected to <paramref name="vertex"/></returns>
        public LinkedList<Vertex> GetConnectedVertices(Vertex vertex)
        {
            LinkedList<Vertex> connectedVertices = new();
            for (LinkedListNode<Vertex> vrtx = vertices.First; vrtx != null; vrtx = vrtx.Next)
            {
                if (vertex.ConnectedVertexIDs.Contains(vrtx.Data.ID))
                    connectedVertices.Add(new(vrtx.Data));
            }
            return connectedVertices;
        }

        /// <summary>
        /// Get IDs of vertices connected to <paramref name="pos"/>
        /// </summary>
        /// <returns>List of IDs connected to <paramref name="pos"/></returns>
        public LinkedList<char> GetConnectedIDs(Coordinate pos)
        {
            return new(map[pos].Connections);
        }

        /// <summary>
        /// Get a copy of vertices in the graph
        /// </summary>
        /// <returns>List of vertices in the graph</returns>
        public LinkedList<Vertex> GetListOfVertices()
        {
            return Copy.VetrexList(vertices);
        }

        /// <summary>
        /// Get IDs of vertices in the graph
        /// </summary>
        /// <returns>List of IDs in the graph</returns>
        public LinkedList<char> GetListOfVertexIDs()
        {
            LinkedList<char> IDs = new();
            for (LinkedListNode<Vertex> vrtx = vertices.First; vrtx != null; vrtx = vrtx.Next)
            {
                IDs.Add(vrtx.Data.ID);
            }
            return IDs;
        }

        /// <summary>
        /// Convert a coordinate to an ID
        /// </summary>
        /// <returns>A vertex ID</returns>
        public char CoordinateToID(Coordinate coord)
        {
            return map[coord].ID;
        }

        /// <summary>
        /// Convert an ID to a vertex
        /// </summary>
        /// <returns>A vertex with the <paramref name="ID"/></returns>
        public Vertex IDToVertex(char ID)
        {
            for (LinkedListNode<Vertex> vrtx = vertices.First; vrtx != null; vrtx = vrtx.Next)
            {
                if (vrtx.Data.ID == ID)
                    return new(vrtx.Data);
            }
            return null;
        }

        /// <summary>
        /// Parse the map.txt to vertices and connections
        /// </summary>
        /// <exception cref="InvalidOperationException">When an ID listed as connected but cannot find in the vertices</exception>
        /// <param name="mapLines">Lines of map.txt</param>
        private void ParseMap(string[] mapLines)
        {
            int vertexCount = int.Parse(mapLines[0]);

            for (int i = 1; i <= vertexCount; i++) // skip first line
            {
                Vertex newVertex = new();
                newVertex.ID = mapLines[i][0];
                vertices.Add(newVertex);
            }

            int currentLine = 1;
            for (LinkedListNode<Vertex> vertex = vertices.First; vertex != null; vertex = vertex.Next)
            {

                for (int j = 1; j < mapLines[currentLine].Length; j++) // skip first character
                {
                    LinkedListNode<Vertex> connectedVertex = vertices.First;
                    while (connectedVertex != null && connectedVertex.Data.ID != mapLines[currentLine][j])
                    {
                        connectedVertex = connectedVertex.Next;
                    }

                    if (connectedVertex != null)
                    {
                        vertex.Data.ConnectedVertexIDs.Add(connectedVertex.Data.ID);
                    }
                    else
                        throw new InvalidOperationException($"Vertex: '{mapLines[currentLine][j]}' does not exist on the map.");
                }
                currentLine++;
            }
        }

        /// <summary>
        /// Try to generate the Map an fill the BlockBuffer. If failed disable the Map.
        /// </summary>
        private void GenerateGraphLayout()
        {
            LinkedList<Vertex> longestPath = SearchLongestPath();
            AddTheHorizontalVertices(longestPath);

            LinkedList<Vertex> missedOut = GenerateMissedVertices(longestPath);

            bool canContinue = AddMissedVertices(missedOut);
            if (!canContinue)
            {
                FillPlaceholder();
                return;
            }

            map.AddBorder(3);
            canContinue = GenerateMapPaths();
            if (!canContinue)
            {
                //retry with larger map
                MakeTheMapSquare();

                canContinue = GenerateMapPaths();
                if (!canContinue)
                {
                    FillPlaceholder();
                    return;
                }
            }

            FillBlockBuffer(Array.Empty<MapItemColor>());
        }

        /// <summary>
        /// Finds the longest, continuous, loop-free path between the vertices with the help of the Depth-First-Search algorithm
        /// </summary>
        /// <returns>List of vertices of the longest path</returns>
        private LinkedList<Vertex> SearchLongestPath()
        {
            LinkedList<Vertex> path = new();
            LinkedList<Vertex> verticesWithLeastConns = VerticesWithLeastConnections(vertices);
            for (LinkedListNode<Vertex> vertex = verticesWithLeastConns.First; vertex != null; vertex = vertex.Next)
            {
                LinkedList<Vertex> longestPath = new();
                DFS(vertex.Data, ref longestPath, new(), new());
                if (path.Length < longestPath.Length)
                    path = longestPath;
            }

            return path;
        }

        /// <summary>
        /// A modified version of Depth-First-Search algorithm.
        /// The difference is this can return the longest traversed path and 
        /// when going "backwards" it removes the node from the discovered nodes list
        /// </summary>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Depth-first_search#Pseudocode
        /// </remarks>
        /// <param name="start">The Vertex where to start from</param>
        /// <param name="longestPath">Reference to the container of the longest path. 
        ///                           It can be used safely outside of this method</param>
        /// <param name="discovered">Used by the function internally. Stores the list of discovered nodes</param>
        /// <param name="path">Used by the function internally. Stores the path we are currently on</param>
        private void DFS(Vertex start, ref LinkedList<Vertex> longestPath, LinkedList<Vertex> discovered = null, LinkedList<Vertex> path = null)
        {
            if (discovered == null)
                discovered = new();

            if (path == null)
                path = new();

            if (discovered.Length == 0)
                path.Add(start);

            for (LinkedListNode<char> NeighbourID = start.ConnectedVertexIDs.First; NeighbourID != null; NeighbourID = NeighbourID.Next)
            {
                if (FindVertexInList(discovered, NeighbourID.Data) == null)
                {
                    Vertex connected = FindVertexInList(vertices, NeighbourID.Data);
                    Vertex newPathNode = new(connected);
                    path.Add(newPathNode);

                    discovered.Add(start);
                    DFS(connected, ref longestPath, discovered, path);

                    if (path.Length > longestPath.Length)
                    {
                        longestPath = Copy.VetrexList(path);
                    }

                    path.PopBack();
                    discovered.PopBack();
                }
            }
        }

        /// <summary>
        /// Add the <paramref name="longestPath"/> as a horizontal line of vertices to the map
        /// </summary>
        /// <param name="longestPath">The longest path returned by <c>SearchLongestPath()</c></param>
        private void AddTheHorizontalVertices(LinkedList<Vertex> longestPath)
        {
            int col = 0;
            for (LinkedListNode<Vertex> vertex = longestPath.First; vertex != null; vertex = vertex.Next)
            {
                if (col == 0)
                {
                    map.AddRowBottom();
                }
                else
                {
                    map.AddColRight();
                    map[0, col] = new MapItem(PathSegment.Horizontal);
                    col++;
                }

                LinkedList<char> missingConns = new(vertex.Data.ConnectedVertexIDs);

                if (col >= 2)
                {
                    char thisID = vertex.Data.ID;
                    char prevID = vertex.Prev.Data.ID;
                    missingConns.Remove(prevID);
                    map[0, col - 2].MissingConnections.Remove(thisID);
                }

                map.AddColRight();
                map[0, col] = new MapItem(vertex.Data.ID, vertex.Data.ConnectedVertexIDs, missingConns);
                col++;
            }
            //MakeTheMapSquare();
        }

        /// <summary>
        /// Appends the required amount of rows to make the map square
        /// </summary>
        private void MakeTheMapSquare()
        {
            int neededRows = map.Cols - map.Rows;
            int neededCols = map.Rows - map.Cols;
            if (neededRows > 0)
            {
                map.AddRowTop(neededRows / 2);
                map.AddRowBottom(neededRows / 2);
            }
            else if(neededCols > 0)
            {
                map.AddColLeft(neededCols / 2);
                map.AddColRight(neededCols / 2);
            }
        }

        /// <summary>
        /// Finds the vertices that cannot be found on the longest path
        /// </summary>
        /// <param name="longestPath"></param>
        /// <returns>List of vertices that not on the longest path</returns>
        private LinkedList<Vertex> GenerateMissedVertices(LinkedList<Vertex> longestPath)
        {
            LinkedList<Vertex> missedOut = Copy.VetrexList(vertices);
            for (LinkedListNode<Vertex> vertex = longestPath.First; vertex != null; vertex = vertex.Next)
            {
                missedOut.Remove(vertex.Data);
            }
            return missedOut;
        }

        /// <summary>
        /// Add vertices to the Map which are not on the longest path
        /// </summary>
        /// <param name="missedVertices">List of missed vertices</param>
        /// <returns>True of all vertices added successfully. False otherwise</returns>
        private bool AddMissedVertices(LinkedList<Vertex> missedVertices)
        {
            int verticalOffset = 2;
            int lastLen = int.MaxValue;
            while (missedVertices.Length > 0 && missedVertices.Length < lastLen)
            {
                lastLen = missedVertices.Length;

                for (LinkedListNode<Vertex> vertex = missedVertices.First; vertex != null; vertex = vertex.Next)
                {
                    LinkedList<char> connVertices = vertex.Data.ConnectedVertexIDs;
                    int xSum = 0;
                    int yEdge = 0;
                    int onMapCount = 0;
                    if (verticalOffset < 0)
                        yEdge = int.MaxValue;

                    bool leastOneOnMap = false;
                    LinkedListNode<char> connectedVertex = connVertices.First;
                    while (connectedVertex != null)
                    {
                        Coordinate tmp = map.CoordOf(connectedVertex.Data);
                        if (tmp.X != -1)
                        {
                            leastOneOnMap = true;
                            onMapCount++;
                            xSum += tmp.X;
                            if (verticalOffset > 0 && yEdge < tmp.Y)
                                yEdge = tmp.Y;
                            else if (verticalOffset < 0 && yEdge > tmp.Y)
                                yEdge = tmp.Y;
                        }

                        connectedVertex = connectedVertex.Next;
                    }

                    if (leastOneOnMap)
                    {
                        Coordinate pos = new();
                        pos.X = xSum / onMapCount;
                        pos.Y = yEdge;
                        pos.Y += verticalOffset;

                        while (map.HasCoordinate(pos) == false)
                        {
                            map.AddRowBottom(2);
                        }

                        if (map[pos].Status != Status.Empty)
                        {
                            pos.Y -= verticalOffset;

                            Coordinate checkPos = new(pos);
                            checkPos.Y += 2;
                            if (map.HasCoordinate(checkPos) == false)
                                map.AddRowBottom(2);

                            checkPos = new(pos);
                            checkPos.Y -= 2;
                            if (map.HasCoordinate(checkPos) == false)
                            {
                                map.AddRowTop(2);
                                pos.Y += 2;
                            }

                            checkPos = new(pos);
                            checkPos.X += 2;
                            if (map.HasCoordinate(checkPos) == false)
                                map.AddColRight(2);

                            checkPos = new(pos);
                            checkPos.X -= 2;
                            if (map.HasCoordinate(checkPos) == false)
                            {
                                map.AddColLeft(2);
                                pos.X += 2;
                            }

                            pos = FindEmptySpace(pos);
                            if (pos.X == -1)
                                return false;
                        }

                        map[pos].Status = Status.Vertex;
                        map[pos].ID = vertex.Data.ID;
                        map[pos].MissingConnections = new(vertex.Data.ConnectedVertexIDs);
                        map[pos].Connections = new(vertex.Data.ConnectedVertexIDs);

                        missedVertices.RemoveByNode(vertex);
                    }
                }
            }
            return missedVertices.Length == 0;
        }

        /// <summary>
        /// Find an empty space on the Map near the <paramref name="coord"/>
        /// </summary>
        /// <param name="coord">The center of the area</param>
        /// <returns>Coordinate of the free space. If there is none (-1,-1) returned</returns>
        private Coordinate FindEmptySpace(Coordinate coord)
        {
            int offset = 2;
            Coordinate[] offsets =
            {
                new( offset, 0),      // down
                new(-offset, 0),      // up
                new( 0,      offset), // right
                new( 0,     -offset), // left
                new(-offset,-offset), // up & left
                new(-offset, offset), // up & right
                new( offset,-offset), // down & left
                new( offset, offset), // down & right
            };

            for (int i = 0; i < offsets.Length; i++)
            {
                Coordinate c = offsets[i] + coord;
                if (map.HasCoordinate(c))
                {
                    if (map[c].Status == Status.Empty)
                        return c;
                }
            }

            return new(-1, -1);
        }

        /// <summary>
        /// Try to connect the vertices on the map with Dijkstra's path-finding algorithm<br/>
        /// <br/>
        /// The closest connections are tried first then the cost limit is gradually increased
        /// to allow the most distant connections to be made.
        /// </summary>
        /// <remarks></remarks>
        /// <returns>True if all vertices connected successfully. False otherwise</returns>
        private bool GenerateMapPaths()
        {
            LinkedList<MapItem> mapVertices = map.GetListOfVerticesByRef();
            bool allConnected = false;
            int costLimit = 2;
            int maxCost = Math.Min((map.Cols + map.Rows) * 2, 10000); // perimeter or 10000
            while (!allConnected && costLimit < maxCost)
            {
                bool allConnectedInLoop = true;
                LinkedListNode<MapItem> mapitem = mapVertices.First;
                while (mapitem != null)
                {
                    if (mapitem.Data.MissingConnections.Length > 0)
                    {
                        allConnectedInLoop = false;

                        ProcessMissingConnections(mapitem.Data, costLimit);

                    }

                    mapitem = mapitem.Next;
                }

                if (allConnectedInLoop)
                    allConnected = true;

                costLimit += 4;
            }

            return allConnected;
        }

        /// <summary>
        /// Try to connect the vertices on the map with Dijkstra's path-finding algorithm
        /// </summary>
        /// <param name="mapitem">Try to connect missing connections of this item</param>
        /// <param name="costLimit">Limit the cost of th path</param>
        private void ProcessMissingConnections(MapItem mapitem, int costLimit)
        {
            LinkedList<char> missingConns = mapitem.MissingConnections;
            LinkedListNode<char> missingConn = missingConns.First;

            while (missingConn != null)
            {
                Coordinate targetCoord = map.CoordOf(missingConn.Data);
                LinkedList<DijkstraInfo> dijkstraInfos = Dijkstra(mapitem, targetCoord, costLimit);

                LinkedListNode<DijkstraInfo> targetInfo = dijkstraInfos.Find(new(targetCoord));

                if (targetInfo != null)
                {
                    if (targetInfo.Data.Cost <= costLimit)
                    {
                        LinkedList<Coordinate> path = BacktrackDijkstraPath(dijkstraInfos, targetInfo.Data);

                        LinkedListNode<Coordinate> coord = path.First.Next; // start from the second because the first is a vertex

                        while (coord != null)
                        {
                            if (map[coord.Data].Status == Status.Empty)
                            {
                                map[coord.Data].PathSegment = CalcOptimalPathSegment(coord);
                                map[coord.Data].Status = Status.Path;
                            }
                            coord = coord.Next;
                        }

                        map[targetCoord].MissingConnections.Remove(mapitem.ID);
                        missingConns.RemoveByNode(missingConn);
                    }

                }

                missingConn = missingConn.Next;
            }
        }

        /// <summary>
        /// The Dijkstra's path-finding algorithm
        /// </summary>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Dijkstra's_algorithm#Pseudocode
        /// </remarks>
        /// <param name="start">MapItem to start from</param>
        /// <param name="target">Coordinate to be reached</param>
        /// <param name="costLimit">Limit the cost. We look no further if we reached this value </param>
        /// <returns>The discovered paths table. Only those that have lower or equal cost than <paramref name="costLimit"/> can be found in it</returns>
        private LinkedList<DijkstraInfo> Dijkstra(MapItem start, Coordinate target, int costLimit)
        {
            Coordinate startCoord = map.CoordOf(start);
            LinkedList<DijkstraInfo> VisitedPathCostTable = new();
            LinkedList<DijkstraInfo> UnvisitedPathCostTable = GenerateUnvisitedTable(startCoord);

            bool targetReached = false;
            bool costLimitReached = false;
            while (!targetReached && !costLimitReached && UnvisitedPathCostTable.Length > 0)
            {
                DijkstraInfo current = CheapestNode(UnvisitedPathCostTable);

                if (current.Cost > costLimit)
                    costLimitReached = true;
                else
                {
                    LinkedList<Coordinate> adjacentCoords = map.AdjacentCoordinatesOf(current.Coord);
                    for (LinkedListNode<Coordinate> adjCoord = adjacentCoords.First; adjCoord != null; adjCoord = adjCoord.Next)
                    {
                        bool isInVisited = true;
                        LinkedListNode<DijkstraInfo> UnvisitedTableNode = UnvisitedPathCostTable.First;
                        while (isInVisited == true && UnvisitedTableNode != null)
                        {
                            if (adjCoord.Data.Equals(UnvisitedTableNode.Data.Coord))
                                isInVisited = false;

                            if (isInVisited == true)
                                UnvisitedTableNode = UnvisitedTableNode.Next;
                        }

                        if (isInVisited == false)
                        {
                            int adjacentNodeCost = 0;
                            if (adjCoord.Data.Equals(target) == false) // ignore the cost if it's the target
                                adjacentNodeCost = map[adjCoord.Data].Cost;

                            DijkstraInfo UnvisitedNode = UnvisitedTableNode.Data;
                            int currentCost = current.Cost + adjacentNodeCost;
                            currentCost += CalcAdditionalDirectionCost(current.Coord, UnvisitedNode.Coord);

                            if (currentCost < UnvisitedNode.Cost)
                            {
                                UnvisitedNode.Cost = currentCost;
                                UnvisitedNode.PreviousCoord = current.Coord;
                            }
                        }
                    }

                    UnvisitedPathCostTable.Remove(current);
                    VisitedPathCostTable.Add(current);

                    if (current.Coord.Equals(target))
                        targetReached = true;
                }

            }
            return VisitedPathCostTable;
        }

        /// <summary>
        /// Generate the initial Path-Cost table. Set the start coordinate's cost to 0.
        /// </summary>
        /// <param name="startCoord">The starting coordinate</param>
        /// <returns>The generated Path-Cost table</returns>
        private LinkedList<DijkstraInfo> GenerateUnvisitedTable(Coordinate startCoord)
        {
            LinkedList<DijkstraInfo> UnvisitedPathCostTable = new();
            for (int row = 0; row < map.Rows; row++)
            {
                for (int col = 0; col < map.Cols; col++)
                {
                    UnvisitedPathCostTable.Add(new DijkstraInfo(new Coordinate(row, col)));
                    if (row == startCoord.Y && col == startCoord.X)
                        UnvisitedPathCostTable.Last.Data.Cost = 0;
                }
            }
            return UnvisitedPathCostTable;
        }

        /// <summary>
        /// Find the cheapest node in a Dijkstra path-cost table
        /// </summary>
        /// <param name="pathCostTable">Table to be searched</param>
        /// <returns>First cheapest node</returns>
        private DijkstraInfo CheapestNode(LinkedList<DijkstraInfo> pathCostTable)
        {
            LinkedListNode<DijkstraInfo> cheapest = pathCostTable.First;
            for (LinkedListNode<DijkstraInfo> di = pathCostTable.First; di != null; di = di.Next)
            {
                if (di.Data.Cost < cheapest.Data.Cost)
                    cheapest = di;
            }
            return cheapest.Data;
        }

        /// <summary>
        /// Calculate the additional cost based on the relation of coordinates to each other
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>The additional cost</returns>
        private int CalcAdditionalDirectionCost(Coordinate from, Coordinate to)
        {
            int distY = Math.Abs(from.Y - to.Y);
            int distX = Math.Abs(from.X - to.X);
            if (distY == 1 && distX == 1) // diagonal
                return 2;
            else if (distY == 1 || distX == 1) // vertical / horizontal
                return 1;
            else // not an adjacent coordinate
                return 50000;
        }

        /// <summary>
        /// Generates the path based on the list of DijkstraInfos from the end to the start
        /// </summary>
        /// <param name="dijkstraInfos">Database of the Dijkstra algorithm</param>
        /// <param name="from">The end node of the path</param>
        /// <returns>The coordinates of the path elements in reverse order. The first element is the end of the path and the last element is the start</returns>
        private LinkedList<Coordinate> BacktrackDijkstraPath(LinkedList<DijkstraInfo> dijkstraInfos, DijkstraInfo from)
        {
            LinkedList<Coordinate> path = new();
            Coordinate coord = from.Coord;
            while (coord != null)
            {
                path.Add(coord);
                coord = dijkstraInfos.Find(new(coord)).Data.PreviousCoord;
            }
            return path;
        }

        /// <summary>
        /// Calculates the optimal type of PathSegment based on the surrounding places
        /// </summary>
        /// <param name="coord">Coordinate of the place in question</param>
        /// <returns>The calculated PathSegment</returns>
        private PathSegment CalcOptimalPathSegment(LinkedListNode<Coordinate> coord)
        {
            Coordinate center = coord.Data;
            Coordinate test = coord.Prev.Data;
            uint segmentCode = 0;
            if (coord.Next != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    int diffY = test.Y - center.Y;
                    int diffX = test.X - center.X;
                    if (diffY == -1 && diffX == -1) // top left
                        segmentCode ^= 0x1100;
                    else if (diffY == -1 && diffX == 0) // top center
                        segmentCode ^= 0x0200;
                    else if (diffY == -1 && diffX == 1) // top right
                        segmentCode ^= 0x0310;
                    else if (diffY == 0 && diffX == -1) // center left
                        segmentCode ^= 0x2000;
                    else if (diffY == 0 && diffX == 1) // center right
                        segmentCode ^= 0x0020;
                    else if (diffY == 1 && diffX == -1) // bottom left
                        segmentCode ^= 0x3001;
                    else if (diffY == 1 && diffX == 0) // bottom center
                        segmentCode ^= 0x0002;
                    else if (diffY == 1 && diffX == 1) // bottom right
                        segmentCode ^= 0x0033;

                    if (coord.Next != null)
                        test = coord.Next.Data;
                }
            }
            return (PathSegment)segmentCode;
        }

        /// <summary>
        /// Fills the BlockBuffer with the Map and the colors of the Map
        /// </summary>
        /// <param name="mColor">Array of the MapItem colors</param>
        private void FillBlockBuffer(MapItemColor[] mColor)
        {
            Dimension mapDimension = new(map.Rows, map.Cols);
            Dimension blockDimension = new(3, 5);
            bf = new(mapDimension, blockDimension);

            for (int row = 0; row < map.Rows; row++)
            {
                for (int col = 0; col < map.Cols; col++)
                {
                    MapItem item = map[row, col];
                    if (item.Status == Status.Path)
                    {
                        MapItemColor mapColor = FindMapColorAtCoord(mColor, new(row, col));
                        if (mapColor != null)
                        {
                            bf.Blocks[row, col].BackgroundColor = mapColor.BackgroundColor;
                            bf.Blocks[row, col].ForegroundColor = mapColor.ForegroundColor;
                        }
                        bf.Blocks[row, col].Data = Sprite.PathSegmentToSprite(item.PathSegment);
                    }
                    else if (item.Status == Status.Vertex)
                    {
                        MapItemColor mapColor = FindMapColorAtCoord(mColor, new(row, col));
                        if (mapColor != null)
                        {
                            bf.Blocks[row, col].BackgroundColor = mapColor.BackgroundColor;
                            bf.Blocks[row, col].ForegroundColor = mapColor.ForegroundColor;
                        }
                        bf.Blocks[row, col].Data = Sprite.VertexToSprite(item.ID);
                    }
                }
            }
        }

        /// <summary>
        /// Converts VertexColor to MapItemColor
        /// </summary>
        /// <param name="vrtxColor">List of VertexColors</param>
        /// <returns>An array of MapItemColors</returns>
        private MapItemColor[] ColoredVrtxToMapColor(List<VertexColor> vrtxColor)
        {
            MapItemColor[] mcs = new MapItemColor[vrtxColor.Length];
            for (int i = 0; i < vrtxColor.Length; i++)
            {
                Coordinate coord = map.CoordOf(vrtxColor[i].ID);
                MapItemColor mc = new(coord, vrtxColor[i].ForegroundColor, vrtxColor[i].BackgroundColor);
                mcs[i] = mc;
            }

            return mcs;
        }

        /// <summary>
        /// Disables the Map and adds the placeholder to the BlockBuffer
        /// </summary>
        private void FillPlaceholder()
        {
            generateMap = false;
            bf = Sprite.GetPlaceholder();
        }

        /// <summary>
        /// Find the map color at <paramref name="coord"/>
        /// </summary>
        /// <param name="mapColors">Array of MapItemColors</param>
        /// <param name="coord">The coordinate</param>
        /// <returns>The MapItemColor or null of cannot be found</returns>
        private MapItemColor FindMapColorAtCoord(MapItemColor[] mapColors, Coordinate coord)
        {
            for (int i = 0; i < mapColors.Length; i++)
            {
                if (mapColors[i].Coordinate.Equals(coord))
                {
                    return mapColors[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Finds a vertex in the <paramref name="list"/> which has the <paramref name="ID"/>
        /// </summary>
        /// <param name="list">A Vertex list</param>
        /// <param name="ID">A Vertex ID</param>
        /// <returns>The vertex with the <paramref name="ID"/> or null if cannot be found</returns>
        private Vertex FindVertexInList(LinkedList<Vertex> list, char ID)
        {
            for (LinkedListNode<Vertex> node = list.First; node != null; node = node.Next)
            {
                if (node.Data.ID == ID)
                {
                    return node.Data;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds vertices which cave the least amount of connection to other vertices
        /// </summary>
        /// <param name="vertexList">A list of Vertices</param>
        /// <returns>The vertices which have the least amount of connections</returns>
        private LinkedList<Vertex> VerticesWithLeastConnections(LinkedList<Vertex> vertexList)
        {
            LinkedList<Vertex> vericesWLeastConns = new();
            int minCount = int.MaxValue;
            for (LinkedListNode<Vertex> vertex = vertexList.First; vertex != null; vertex = vertex.Next)
            {
                int connCount = vertex.Data.ConnectedVertexIDs.Length;
                if (vericesWLeastConns.Length == 0)
                {
                    vericesWLeastConns.Add(new(vertex.Data));
                    minCount = connCount;
                }
                else if (minCount > connCount)
                {
                    vericesWLeastConns.Clear();
                    vericesWLeastConns.Add(new(vertex.Data));
                    minCount = connCount;
                }
                else if (minCount == connCount)
                {
                    vericesWLeastConns.Add(new(vertex.Data));
                }
            }
            return vericesWLeastConns;
        }
    }
}
