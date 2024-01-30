namespace Solvedoku
{
    using System;
    using System.Diagnostics;
    using TableArray = int[,];

    public static class Exts
    {
        public static void ScanGridAtCoord(this List<int> list, ref int[,] table, int x, int y)
        {
            int startingNum = table[x, y];
            List<Tuple<int, int>> possibleCells = new();
            List<int> possibleNumbers = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // remove current grid numbers
            int gridStartX = (int)Math.Floor(x / 3d) * 3;
            int gridStartY = (int)Math.Floor(y / 3d) * 3;
            for (int gridX = gridStartX; gridX < gridStartX + 3; ++gridX)
            {
                for (int gridY = gridStartY; gridY < gridStartY + 3; ++gridY)
                {
                    ref int gridXY = ref table[gridX, gridY];
                    if (gridXY == 0)
                    {
                        possibleCells.Add(new(gridX, gridY));
                        continue;
                    }

                    possibleNumbers.Remove(gridXY);
                }
            }

            var numbersCopy = new List<int>(possibleNumbers);
            foreach (var num in numbersCopy)
            {
                var cellsCopy = new List<Tuple<int, int>>(possibleCells);
                var cellsCopyCopy = new List<Tuple<int, int>>(cellsCopy);
                foreach (var coord in cellsCopyCopy)
                {
                    // check on X and Y axis
                    for (int check = 0; check < 9; ++check)
                    {
                        ref int numXaxis = ref table[coord.Item1, check];
                        ref int numYaxis = ref table[check, coord.Item2];

                        if ((numXaxis == 0 || num != numXaxis) && (numYaxis == 0 || num != numYaxis))
                            continue;

                        cellsCopy.Remove(coord);

                        if (cellsCopy.Count <= 1)
                            break;
                    }

                    if (cellsCopy.Count <= 1)
                        break;
                }

                // only one cell remaining that could fit the number currently in loop
                if (cellsCopy.Count == 1)
                {
                    if (cellsCopy.ElementAt(0) is Tuple<int, int> tuple && tuple.Item1 == x && tuple.Item2 == y)
                    {
                        list.Clear();
                        list.Add(num);
                        return;
                    }
                }
            }

            return;
        }
    }

    internal class Program
    {
        static void PrintTable(ref TableArray table)
        {
            Console.Clear();
            Console.WriteLine("---1--2--3-|-4--5--6-|-7--8--9");
            for (int x = 0; x < 9; ++x)
            {
                if (x % 3 == 0 && x > 0)
                    Console.WriteLine("   ---------------------------");

                Console.Write($"{x + 1} ");
                for (int y = 0; y < 9; ++y)
                {
                    if (y % 3 == 0 && y > 0)
                        Console.Write("|");

                    Console.Write($" {table[x, y]} ");
                }

                Console.Write("\n");
            }
        }

        static void Main(string[] args)
        {
            TableArray table =
            {
                { 5, 3, 0, 0, 7, 0, 0, 0, 0 },
                { 6, 0, 0, 1, 9, 5, 0, 0, 0 },
                { 0, 9, 8, 0, 0, 0, 0, 6, 0 },
                { 8, 0, 0, 0, 6, 0, 0, 0, 3 },
                { 4, 0, 0, 8, 0, 3, 0, 0, 1 },
                { 7, 0, 0, 0, 2, 0, 0, 0, 6 },
                { 0, 6, 0, 0, 0, 0, 2, 8, 0 },
                { 0, 0, 0, 4, 1, 9, 0, 0, 5 },
                { 0, 0, 0, 0, 8, 0, 0, 7, 9 },
            };

            /*string? input;
            while (true)
            {
                input = Console.ReadLine();

                if (input == null || input.ToLower().ElementAtOrDefault(0) == 's')
                    break;

                *//*Int32.TryParse(input, out var x);
                Int32.TryParse(input, out var y);
                Int32.TryParse(input, out var value);

                table[x,y] = value;*//*
            }*/

            PrintTable(ref table);

            Console.ReadLine();

            Console.WriteLine("Generating numbers...");

            TableArray smallGrid = new int[3, 3];

            Stopwatch timer = new();
            timer.Start();

            while (true)
            {
                bool done = true;
                for (int x = 0; x < 9; ++x)
                {
                    for (int y = 0; y < 9; ++y)
                    {
                        if (table[x, y] != 0)
                            continue;

                        done = false;
                        List<int> possibleNumbers = new(){ 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                        // check on X and Y axis
                        // scan in all directions
                        for (int check = 0; check < 9; ++check)
                        {
                            ref int numXaxis = ref table[x, check];
                            ref int numYaxis = ref table[check, y];

                            if (numXaxis != 0)
                                possibleNumbers.Remove(numXaxis);

                            if (numYaxis != 0)
                                possibleNumbers.Remove(numYaxis);
                        }

                        // scan all empty boxes for in grid for remaining numbers
                        if (possibleNumbers.Count != 1)
                        {
                            if ((x + 1) % 3 == 0 || (y + 1) % 3 == 0)
                                possibleNumbers.ScanGridAtCoord(ref table, x, y);
                        }

                        if (possibleNumbers.Count == 1)
                        {
                            Console.WriteLine($"Found number {possibleNumbers.ElementAt(0)}; X {x + 1} Y {y + 1}");
                            table[x, y] = possibleNumbers.ElementAt(0);

                            PrintTable(ref table);
                        }
                    }
                }

                if (done)
                    break;

                Thread.Sleep(10);
            }

            timer.Stop();
            Console.WriteLine($"Solved in {timer}.");
            Console.ReadLine();
        }
    }
}
