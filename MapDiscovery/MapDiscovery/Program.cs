using System.Drawing;

namespace MapDiscovery
{
    internal class Program
    {
        const int range = 10;
        static int dimensions;
        static Point pos, lastPos;
        static int columns, rows;
        static Field[,] map;
        static bool quitted;

        static void Main(string[] args)
        {
            while (true)
            {
                Initialize();

                while (!quitted)
                {
                    GetNewPos();
                    UpdateMap();
                }
            }
        }

        private static void Draw(int r, int c)
        {
            Console.CursorTop = r;
            Console.CursorLeft = c;

            switch (map[r, c].Value)
            {
                case 0:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("?");
                    break;
                case 1:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("&");
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("%");
                    break;
            }
        }

        private static void GetNewPos()
        {
            lastPos = pos;
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.W && pos.Y > 0)
                pos.Y--;

            else if (key.Key == ConsoleKey.S && pos.Y < rows - 1)
                pos.Y++;

            else if (key.Key == ConsoleKey.A && pos.X > 0)
                pos.X--;

            else if (key.Key == ConsoleKey.D && pos.X < columns - 1)
                pos.X++;

            else if (key.Key == ConsoleKey.R)
                quitted = true;
        }

        private static void Initialize()
        {
            Console.Clear();
            DisplayInfo();
            SetVariables();
            GetMapSize();
            GenerateMap();
            UpdateMap();
        }

        private static void DisplayInfo()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("INFO: Press 'R' to reset the map during runtime.\n");
        }

        private static void SetVariables()
        {
            pos = new Point(0, 0);
            quitted = false;
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void GetMapSize()
        {
            Console.WriteLine("Enter size of the map:");
            bool valid = false;
            while (valid == false)
            {
                if (int.TryParse(Console.ReadLine(), out dimensions))
                {
                    try
                    {
                        columns = rows = Console.BufferWidth = Console.BufferHeight = dimensions;
                        valid = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(Environment.NewLine + e.Message + " Try again:");
                    }
                }
                else
                    Console.WriteLine("\nNot a valid number. Try again:");
            }
            Console.Clear();
        }

        private static void UpdateMap()
        {
            for (int r = pos.Y - range / 2; r < pos.Y + range / 2; r++)
            {
                for (int c = pos.X - range / 2; c < pos.X + range / 2; c++)
                {
                    if (r >= 0 && r < rows && c >= 0 && c < columns)
                    {
                        if (map[r, c].IsDiscovered == false && Math.Abs(Math.Pow(pos.X - c, 2) + Math.Pow(pos.Y - r, 2)) < range)
                        {
                            map[r, c].IsDiscovered = true;
                            Draw(r, c);
                        }
                    }
                }
            }
            DrawPlayer();
        }

        private static void DrawPlayer()
        {
            Console.CursorTop = pos.Y;
            Console.CursorLeft = pos.X;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("#");

            if (lastPos != pos)
                Draw(lastPos.Y, lastPos.X); //Override # from the previous spot
        }

        private static void GenerateMap()
        {
            Console.WriteLine("Generating map...");
            map = new Field[rows, columns];
            Random random = new();
            Noise.Seed = random.Next();
            var noiseValues = Noise.Calc2D(columns, rows, 0.02f);
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    map[r, c] = new Field();
                    if (noiseValues[c, r] <= 80)
                        map[r, c].Value = 0;

                    else if (noiseValues[c, r] <= 130)
                        map[r, c].Value = 1;

                    else
                        map[r, c].Value = 2;
                }
            }
            Console.Clear();
        }
    }
}