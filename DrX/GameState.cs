using DrX.GraphHopper;
using DrX.Utils.Generic;
using System;

namespace DrX
{
    /// <summary>
    /// Represents the current state of the game.
    /// </summary>
    class GameState
    {
        public bool IsGameEnded { get; private set; } = false;
        public bool IsDrXWin { get; private set; } = true;
        public int VertexCount { get; }
        public int RoundCount { get; private set; } = 0;

        public int PlayerCount
        {
            get { return players.Length; }
        }
        public static int DrXID { get; } = 0;

        private static readonly Random rng = new();
        private readonly List<Player> players = new();
        private readonly Graph gameMap;

        /// <summary></summary>
        /// <param name="gameMap">Graph to be used by the game</param>
        public GameState(Graph gameMap)
        {
            this.gameMap = gameMap;
            VertexCount = this.gameMap.GetListOfVertices().Length;

            Player drX = new("DrX", ConsoleColor.Red, 0);
            drX.Hidden = true;
            AddPlayer(drX);
        }

        /// <summary>
        /// Adds a player to the game.
        /// </summary>
        /// <param name="player">A player instance</param>
        /// <param name="startingPoint">ID of the desired staring point</param>
        public void AddPlayer(Player player, char startingPoint)
        {
            Player newPlayer = new(player);
            newPlayer.Position = gameMap.IDToVertex(startingPoint);
            AddPlayer(newPlayer);
        }

        /// <summary>
        /// Adds a player to the game.
        /// </summary>
        /// <param name="player">A player instance</param>
        public void AddPlayer(Player player)
        {
            Player newPlayer = new(player);
            newPlayer.ID = players.Length;
            players.Add(newPlayer);
        }

        /// <summary>
        /// Get a player based on the ID
        /// </summary>
        /// <param name="ID">The ID of the player</param>
        /// <returns>A copy of the player</returns>
        public Player GetPlayer(int ID)
        {
            return new(players[ID]);
        }

        /// <summary>
        /// Sets a player to active state.
        /// </summary>
        /// <param name="ID">The ID of the player</param>
        /// <param name="active"></param>
        public void SetPlayerActive(int ID, bool active)
        {
            players[ID].Active = active;
        }

        /// <summary>
        /// Find an empty place for DrX and place it there.
        /// </summary>
        public void PlaceDrX()
        {
            LinkedList<Vertex> freePos = GetFreeVertices();
            players[0].Position = freePos[rng.Next(0, freePos.Length)];
        }

        /// <summary>
        /// Moves DrX randomly to a free place
        /// </summary>
        public void MoveDrX()
        {
            if (RoundCount == VertexCount / 4)
            {
                IsGameEnded = true;
                IsDrXWin = true;
            }
            else
            {
                LinkedList<Vertex> freePos = gameMap.GetConnectedVertices(players[DrXID].Position);
                RemoveRealPlayerPositions(freePos);
                if (freePos.Length > 0)
                    players[0].Position = freePos[rng.Next(0, freePos.Length)];
            }

        }

        /// <summary>
        /// Moves a player to the desired position
        /// </summary>
        /// <param name="playerID">The ID of the player</param>
        /// <param name="positionID">The ID of the desired position</param>
        public void MovePlayerTo(int playerID, char positionID)
        {
            Vertex pos = gameMap.IDToVertex(positionID);

            if (pos == null)
                throw new ArgumentException($"PositionID is invalid: {positionID}");

            if (playerID == 0)
            {
                if (GetRealPlayerPositions().Contains(pos))
                    throw new ArgumentException("DrX cannot step on a player owned vertex.");

                players[0].Position = pos;
            }
            else
            {
                if (players[DrXID].Position.Equals(pos)) // if we stepped on DrX's vertex
                {
                    IsGameEnded = true;
                    IsDrXWin = false;
                }
                players[playerID].Position = pos;
            }
        }

        /// <summary>
        /// Increment the round counter
        /// </summary>
        public void NextRound()
        {
            RoundCount++;
        }

        /// <summary>
        /// Get a list of positionIDs where the specified player can step on.
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public LinkedList<char> GetPossibleNextStepIDs(int playerID)
        {
            LinkedList<Vertex> possibleSteps = gameMap.GetConnectedVertices(players[playerID].Position);
            LinkedList<char> stepIDs = new();

            for (LinkedListNode<Vertex> NB = possibleSteps.First; NB != null; NB = NB.Next)
            {
                stepIDs.Add(NB.Data.ID);
            }

            return stepIDs;
        }

        /// <summary></summary>
        /// <returns>A list of all vertex IDs on the map</returns>
        public LinkedList<char> GetVertexIDs()
        {
            LinkedList<Vertex> vertices = gameMap.GetListOfVertices();
            LinkedList<char> ret = new();
            for (LinkedListNode<Vertex> vertex = vertices.First; vertex != null; vertex = vertex.Next)
            {
                ret.Add(vertex.Data.ID);
            }
            return ret;
        }

        /// <summary>
        /// Changes DrX visibility on the map
        /// </summary>
        /// <param name="visible"></param>
        public void ChangeDrXVisibility(bool visible)
        {
            players[0].Hidden = !visible;
        }

        /// <summary>
        /// Get the current visibility of DrX
        /// </summary>
        /// <returns>True if DrX visible. False otherwise.</returns>
        public bool GetDrXVisibility()
        {
            return !players[0].Hidden;
        }

        /// <summary></summary>
        /// <returns>List of all player positions including DrX.</returns>
        public LinkedList<Vertex> GetAllPlayerPositions()
        {
            LinkedList<Vertex> allPos = GetRealPlayerPositions();
            allPos.Add(players[DrXID].Position);
            return allPos;
        }

        /// <summary></summary>
        /// <returns>List of all player positions excluding DrX.</returns>
        public LinkedList<Vertex> GetRealPlayerPositions()
        {
            LinkedList<Vertex> playerPositions = new();
            for (int i = DrXID + 1; i < players.Length; i++) // skip DrX
            {
                playerPositions.Add(players[i].Position);
            }
            return playerPositions;
        }

        /// <summary></summary>
        /// <returns>List of vertices where no real player resides.</returns>
        private LinkedList<Vertex> GetFreeVertices()
        {
            LinkedList<Vertex> freeVertices = gameMap.GetListOfVertices();
            RemoveRealPlayerPositions(freeVertices);
            return freeVertices;
        }

        /// <summary>
        /// Removes the positions of real players from the <paramref name="positions"/> list.
        /// </summary>
        /// <param name="positions">A Vertex list</param>
        private void RemoveRealPlayerPositions(LinkedList<Vertex> positions)
        {
            LinkedList<Vertex> playerPositions = GetRealPlayerPositions();
            for (LinkedListNode<Vertex> pos = positions.First; pos != null; pos = pos.Next)
            {
                if (playerPositions.Contains(pos.Data))
                    positions.RemoveByNode(pos);
            }
        }
    }
}
