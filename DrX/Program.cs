using System;

namespace DrX
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Játéktér előkészítése...");

            bool generateMap = true;
            if (args.Length == 1)
                generateMap = !(args[0] == "nomap");

            Game g = new(generateMap);
            g.Run();
        }
    }
}
