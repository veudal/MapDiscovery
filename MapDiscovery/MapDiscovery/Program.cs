using System.Drawing;

namespace MapDiscovery
{
    internal class Program
    {
        const int range = 6;

        static Point lastPos;
        static Point pos;
        static int columns = 0;
        static int rows = 0;
        static Field[,] map;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("#");
            while (true)
            {
                GetNewPos();
                UpdateMap();
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
                    Console.Write("+");
                    break;
                case 1:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("*");
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("=");
                    break;
            }
        }

        private static void GetNewPos()
        {
            lastPos = pos;
            var key = Console.ReadKey(true);

            if (columns == 0 || rows == 0)
            {
                Initialize();
            }

            if (key.Key == ConsoleKey.W && pos.Y > 0)
                pos.Y--;

            else if (key.Key == ConsoleKey.S && pos.Y < rows - 1)
                pos.Y++;

            else if (key.Key == ConsoleKey.A && pos.X > 0)
                pos.X--;

            else if (key.Key == ConsoleKey.D && pos.X < columns - 1)
                pos.X++;
        }

        private static void Initialize()
        {
            columns = Console.BufferWidth;
            rows = Console.BufferHeight;
            GenerateMap();
        }

        private static void UpdateMap()
        {
            for (int r = pos.Y - range / 2; r < pos.Y + range / 2; r++)
            {
                for (int c = pos.X - range / 2; c < pos.X + range / 2; c++)
                {
                    if (r >= 0 && r < rows && c >= 0 && c < columns)
                    {
                        if (r == pos.Y && c == pos.X)
                        {
                            Console.CursorTop = r;
                            Console.CursorLeft = c;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("#");
                        }
                        else if (map[r, c].IsDiscovered == false && Math.Abs(Math.Pow(pos.X - c, 2) + Math.Pow(pos.Y - r, 2)) < range)
                        {
                            map[r, c].IsDiscovered = true;
                            Draw(r, c);
                        }
                    }
                }
            }
            if (lastPos != pos)
                Draw(lastPos.Y, lastPos.X); //Override # from the previous spot
        }

        private static void GenerateMap()
        {
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
                    {
                        map[r, c].Value = 0;
                    }
                    else if (noiseValues[c, r] <= 130)
                    {
                        map[r, c].Value = 1;
                    }
                    else
                    {
                        map[r, c].Value = 2;
                    }
                }
            }
        }
    }
}