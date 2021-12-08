using DrX.GraphHopper;
using System;

namespace DrX
{
    class Player
    {
        public string Name { get; set; }
        public Vertex Position { get; set; }
        public ConsoleColor Color { get; set; }
        public int ID { get; set; }
        public bool Hidden { get; set; }
        public bool Active { get; set; }

        public Player()
        {
            Name = "NoName";
            Color = ConsoleColor.Blue;
            Position = new();
            Hidden = false;
            Active = false;
        }

        public Player(string name, ConsoleColor color, int ID)
        {
            Name = name;
            Color = color;
            this.ID = ID;

            Position = new();
            Hidden = false;
            Active = false;
        }

        public Player(Player player)
        {
            Name = player.Name;
            Position = new(player.Position);
            Color = player.Color;
            ID = player.ID;
            Hidden = player.Hidden;
            Active = player.Active;
        }
    }
}
