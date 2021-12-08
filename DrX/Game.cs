using DrX.Assets;
using DrX.GraphHopper;
using DrX.GraphHopper.Containers;
using DrX.Graphics;
using DrX.Utils.Generic;
using System;
using System.IO;

namespace DrX
{
    class Game
    {
        private const ConsoleColor activeFgColor = ConsoleColor.White;
        private const ConsoleColor activeBgColor = ConsoleColor.DarkCyan;
        private const ConsoleColor inactiveFgColor = ConsoleColor.Blue;
        private const ConsoleColor inactiveBgColor = ConsoleColor.Black;
        private const ConsoleColor drxFgColor = ConsoleColor.White;
        private const ConsoleColor drxBgColor = ConsoleColor.DarkRed;

        private static readonly CLIGFX cligfx = CLIGFX.GetInstance();
        private readonly GameState gameState;
        private readonly Graph gameMap;


        /// <summary></summary>
        /// <param name="generateMap">Try to generate game map</param>
        public Game(bool generateMap = true)
        {
            gameMap = new Graph(File.ReadAllLines(@"..\..\..\..\map.txt"), generateMap);
            cligfx.SetGraphBuffer(gameMap.GenerateBlockBuffer());
            gameState = new(gameMap);
        }

        /// <summary>
        /// Start the game.
        /// Returns when the game ended
        /// </summary>
        public void Run()
        {
            UpdateGraphBuffer();

            CreatePlayers(1, gameMap.GetListOfVertices().Length / 4);
            gameState.PlaceDrX();

            MainLoop();

            EndGame();
        }

        /// <summary>
        /// Main loop of the game. Controls how the game runs and when ends
        /// </summary>
        private void MainLoop()
        {
            while (gameState.IsGameEnded == false)
            {
                gameState.NextRound();

                if (gameState.RoundCount % 4 == 0 && gameState.RoundCount != 0)
                    gameState.ChangeDrXVisibility(true);
                else
                    gameState.ChangeDrXVisibility(false);

                int currentPlayer = GameState.DrXID + 1;
                while (currentPlayer < gameState.PlayerCount && gameState.IsGameEnded == false)
                {
                    gameState.SetPlayerActive(currentPlayer, true);
                    //DrawFrame();
                    UpdateGraphBuffer();

                    char pos = AskPlayerMovement(currentPlayer);
                    if (pos != ' ')
                        gameState.MovePlayerTo(currentPlayer, pos);

                    gameState.SetPlayerActive(currentPlayer, false);
                    //DrawFrame();
                    UpdateGraphBuffer();

                    currentPlayer++;
                }

                if (gameState.IsGameEnded == false)
                    gameState.MoveDrX();
            }
        }

        /// <summary>
        /// Displays the result of the game
        /// </summary>
        private void EndGame()
        {
            string endGameText = GenStatusText() + "\n\n";
            char drxpos = gameState.GetPlayer(GameState.DrXID).Position.ID;

            if (gameState.IsDrXWin)
                endGameText += Texts.DrXWin.Replace("%drxpos%", drxpos.ToString());
            else
                endGameText += Texts.PlayersWin.Replace("%drxpos%", drxpos.ToString());

            endGameText += Texts.EnterExit;

            endGameText = endGameText.Replace("%round%", gameState.RoundCount.ToString());
            cligfx.SetLineBuffer(endGameText);
            //cligfx.RefreshScreen();
            ReadUserInput();
        }

        /// <summary>
        /// Updates the <c>GraphBuffer</c> with colored data of players
        /// </summary>
        private void UpdateGraphBuffer()
        {
            List<VertexColor> colors = new();

            for (int i = 0; i < gameState.PlayerCount; i++)
            {
                Player p = gameState.GetPlayer(i);
                if (p.Hidden == false)
                {
                    ConsoleColor fg;
                    ConsoleColor bg;

                    if (p.Active)
                    {
                        fg = activeFgColor;
                        bg = activeBgColor;
                    }
                    else if (p.Name == "DrX")
                    {
                        fg = drxFgColor;
                        bg = drxBgColor;
                    }
                    else
                    {
                        fg = inactiveFgColor;
                        bg = inactiveBgColor;
                    }

                    char posID = p.Position.ID;
                    if (p.Active)
                        colors.AddAt(new(posID, fg, bg), 0);
                    else
                        colors.Add(new(posID, fg, bg));
                }
            }

            cligfx.SetGraphBuffer(gameMap.GenerateBlockBuffer(colors));
        }

        /// <summary>
        /// Creates players int the range specified by <paramref name="min"/> and <paramref name="max"/>
        /// </summary>
        /// <param name="min">Minimum number of players (inclusive)</param>
        /// <param name="max">maximum number of players (inclusive)</param>
        private void CreatePlayers(int min, int max)
        {
            string txt = Texts.HowManyPlayers;
            txt = txt.Replace("%min%", min.ToString());
            txt = txt.Replace("%max%", max.ToString());
            cligfx.SetLineBuffer(txt);
            //cligfx.RefreshScreen();

            int playerCount = 0;
            while (playerCount < min || playerCount > max)
            {
                string input = ReadUserInput();
                _ = int.TryParse(input, out playerCount);
            }

            for (int i = 0; i < playerCount; i++)
            {
                Player newPlayer = new();
                newPlayer.Name = AskPlayerName();
                gameState.AddPlayer(newPlayer, AskPlayerPos(newPlayer.Name));
                cligfx.SetLineBuffer(GenPreGameStatusText() + "\n\n" + txt);
                UpdateGraphBuffer();
            }
        }

        /// <summary>
        /// Updates the <c>GraphBuffer</c> and redraws the screen
        /// </summary>
        //private void DrawFrame()
        //{
        //    UpdateGraphBuffer();
        //    cligfx.RefreshScreen();
        //}

        /// <summary>
        /// Asks for a player name
        /// </summary>
        /// <returns>Player name entered by the user</returns>
        private string AskPlayerName()
        {
            cligfx.SetLineBuffer(GenPreGameStatusText() + "\n\n" + Texts.AskName);
            //cligfx.RefreshScreen();
            return ReadUserInput();
        }

        /// <summary>
        /// Asks for an initial player position
        /// </summary>
        /// <returns>ID of chosen starting position</returns>
        private char AskPlayerPos(string playername)
        {
            LinkedList<char> vertexNames = gameState.GetVertexIDs();
            string displayText = Texts.StartPlace;
            displayText = displayText.Replace("%pname%", playername);
            displayText = displayText.Replace("%spos%", IDListToString(vertexNames));
            cligfx.SetLineBuffer(GenPreGameStatusText() + "\n\n" + displayText);
            char answer = '\0';
            while (vertexNames.Contains(answer) == false)
            {
                answer = ReadUserInputToChar();
            }
            return answer;
        }

        /// <summary>
        /// Asks for the next step (staying in place is allowed)
        /// </summary>
        /// <returns>ID of chosen position or ' ' (space) if the player wants to stay in place</returns>
        private char AskPlayerMovement(int playerID)
        {
            LinkedList<char> vertexNames = gameState.GetPossibleNextStepIDs(playerID);

            string displayText = GenStatusText() + "\n\n" + Texts.MovePlayer;

            if (gameState.GetDrXVisibility())
                displayText = GenStatusText() + "\n\n" + Texts.RevealDrX + "\n\n" + Texts.MovePlayer;

            displayText = displayText.Replace("%pname%", gameState.GetPlayer(playerID).Name);
            displayText = displayText.Replace("%spos%", IDListToString(vertexNames));

            char drxpos = gameState.GetPlayer(GameState.DrXID).Position.ID;
            displayText = displayText.Replace("%drxpos%", drxpos.ToString());

            cligfx.SetLineBuffer(displayText);

            char answer = '\0';
            while (vertexNames.Contains(answer) == false && answer != ' ')
            {
                answer = ReadUserInputToChar();
            }
            return answer;
        }

        /// <summary>
        /// Generates the status line from the current state of the game. (only player positions)
        /// </summary>
        /// <returns>String representation of a <c>LineBuffer</c> compatible lines</returns>
        private string GenPreGameStatusText()
        {
            string statusText = Texts.PreGameStatusLine;
            string players = GenPlayerStatus();
            statusText = statusText.Replace("%statustext%", players);

            return statusText;
        }

        /// <summary>
        /// Generates the status line from the current state of the game. (round count and player positions)
        /// </summary>
        /// <returns>String representation of a <c>LineBuffer</c> compatible lines</returns>
        private string GenStatusText()
        {
            string statusText = Texts.StatusLine;
            string players = GenPlayerStatus();
            statusText = statusText.Replace("%statustext%", players);
            statusText = statusText.Replace("%round%", gameState.RoundCount.ToString());

            return statusText;
        }

        /// <summary>
        /// Generates the status line from the current state of the game. (player positions)
        /// </summary>
        /// <returns>Formatted player positions status text</returns>
        private string GenPlayerStatus()
        {
            string players = "";

            for (int i = GameState.DrXID + 1; i < gameState.PlayerCount; i++)
            {
                Player p = gameState.GetPlayer(i);
                if (p.Active)
                    players += string.Format("°b{0}°f{1}", ((int)activeBgColor).ToString("D2"), ((int)activeFgColor).ToString("D2"));
                else
                    players += string.Format("°b{0}°f{1}", ((int)inactiveBgColor).ToString("D2"), ((int)inactiveFgColor).ToString("D2"));

                players += $"{p.Name}: {p.Position.ID}°b--°f--  ";
            }

            return players;
        }

        /// <summary>
        /// Joins a list of IDs to a string separated by ", " (comma and space)
        /// </summary>
        /// <param name="idList">List of IDs</param>
        /// <returns>A string separated by ", " (comma and space)</returns>
        private string IDListToString(LinkedList<char> idList)
        {
            return string.Join(", ", idList.ToArray());
        }

        /// <summary>
        /// Reads a line from the user terminated by an <c>Enter</c>. While reading ensures that the graph panning functions can be used
        /// </summary>
        /// <returns>String inputted by the user</returns>
        private string ReadUserInput()
        {
            string input = "";

            bool end = false;
            while (!end)
            {
                if (cligfx.IsKeyAvailable())
                {
                    ConsoleKeyInfo cki = cligfx.ReadUserInput();

                    if (cki.Key == ConsoleKey.Enter)
                    {
                        end = true;
                        cligfx.ClearInputLine();
                    }
                    else if (cki.Key == ConsoleKey.Backspace)
                    {
                        if (input.Length > 0)
                            input = input.Remove(input.Length - 1);
                    }
                    else if (cki.KeyChar != '\0' && !char.IsControl(cki.KeyChar))
                    {
                        input += cki.KeyChar;
                    }
                }
                else
                    System.Threading.Thread.Sleep(10);

                cligfx.RefreshScreen();
            }
            return input;
        }

        /// <summary>
        /// Reads a char from the user terminated by an <c>Enter</c>. If user types more than a char we try again. While reading ensures that the graph panning functions can be used
        /// </summary>
        /// <returns>Char inputted by the user</returns>
        private char ReadUserInputToChar()
        {
            char answer = '\0';
            while (answer == '\0')
            {
                string input = ReadUserInput();
                if (input.Length == 1)
                    answer = input[0];
            }
            return answer;
        }
    }
}
