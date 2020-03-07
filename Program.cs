using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2dMatrixSolver
{
    class Program
    {
        static int elementsChecked = 0;

        static void Main(string[] args)
        {
            //list of 2-d arrays to solve
            var arraysToSolve = new List<int[,]>();

            arraysToSolve = PopulateArraysList();

            var runTimes = new List<long>();
            foreach (var arr in arraysToSolve)
            {
                //print array to solve 
                Console.WriteLine("Array to solve");
                Console.WriteLine(PrintArray(arr));

                //solve the array and time it
                var watch = System.Diagnostics.Stopwatch.StartNew();
                (Coord, int) ret = Solve(arr);
                watch.Stop();

                var elapsedMs = watch.ElapsedMilliseconds;
                runTimes.Add(elapsedMs);

                Console.WriteLine("Largest Square Coord: " + ret.Item1.Row + ", " + ret.Item1.Col);
                Console.WriteLine("Largest Square Length: " + ret.Item2);
                Console.WriteLine("Took: " + elapsedMs + "ms");
                Console.WriteLine("Elements in array: " + arr.Length);
                Console.WriteLine("Elements checked: " + elementsChecked);
                elementsChecked = 0;

                Console.WriteLine("");
                Console.WriteLine("<--------->");
                Console.WriteLine("");
            }

            try
            {
                int argRowsToSolve = -1;
                int argColsToSolve = -1;
                argRowsToSolve = int.Parse(args[0]);
                argColsToSolve = int.Parse(args[1]);
                int[,] generatedArray = GeneratedArray(argRowsToSolve, argColsToSolve);

                Console.WriteLine("CUSTOM ARRAY REQUEST DETECTED");
                //print array to solve 
                Console.WriteLine("Randomly generated array to solve:");
                Console.WriteLine(PrintArray(generatedArray));

                //solve the array and time it
                var watch = System.Diagnostics.Stopwatch.StartNew();
                (Coord, int) ret = Solve(generatedArray);
                watch.Stop();

                var elapsedMs = watch.ElapsedMilliseconds;
                runTimes.Add(elapsedMs);

                Console.WriteLine("Largest Square Coord: " + ret.Item1.Row + ", " + ret.Item1.Col);
                Console.WriteLine("Largest Square Length: " + ret.Item2);
                Console.WriteLine("Took: " + elapsedMs + "ms");
                Console.WriteLine("Elements in array: " + generatedArray.Length);
                Console.WriteLine("Elements checked: " + elementsChecked);
                elementsChecked = 0;

                Console.WriteLine("");
                Console.WriteLine("<--------->");
                Console.WriteLine("");
            }
            catch
            {
                Console.WriteLine("No valid args detected");
            }
            //print average time it took to solve the arrays
            Console.WriteLine("AVERAGE SOLVE TIME: " + runTimes.Average() + "ms");
        }

        /// <summary>
        /// randomly generates an array of the given dimensions
        /// </summary>
        /// <param name="argRowsToSolve"></param>
        /// <param name="argColsToSolve"></param>
        /// <returns></returns>
        private static int[,] GeneratedArray(int argRowsToSolve, int argColsToSolve)
        {
            int[,] ret = new int[argRowsToSolve, argColsToSolve];
            Random rand = new Random();
            for (int i = 0; i < argRowsToSolve; i++)
            {
                for (int j = 0; j < argColsToSolve; j++)
                {
                    ret[i, j] = rand.Next(0, 2);
                }
            }

            return ret;
        }

        /// <summary>
        /// add some arrays in the format of the ones in this method and see if the algo still works!
        /// </summary>
        /// <returns>a list of 2-d arrays</returns>
        private static List<int[,]> PopulateArraysList()
        {
            var arraysToSolve = new List<int[,]>();

            arraysToSolve.Add(new int[,]
            {
                { 0, 0, 0, 0 },
                { 0, 1, 1, 0 },
                { 0, 1, 1, 0 },
                { 0, 0, 0, 0 }
            });
            arraysToSolve.Add(new int[,]
            {
                { 0, 0, 1 },
                { 0, 0, 1 },
                { 0, 0, 0 }
            });
            arraysToSolve.Add(new int[,]
            {
                { 0, 0, 1 },
                { 0, 0, 1 },
                { 0, 1, 0 }
            });
            arraysToSolve.Add(new int[,]
            {
                { 0, 0, 0, 0 },
                { 1, 1, 0, 0 },
                { 0, 0, 0, 0 },
            });
            arraysToSolve.Add(new int[,]
            {
                { 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1 },
                { 0, 0, 0, 0, 1 },
                { 0, 0, 0, 0, 0 },
                { 1, 1, 0, 1, 1 },
            });
            arraysToSolve.Add(new int[,]
            {
                { 1, 0, 1 },
                { 0, 1, 1 },
                { 1, 0, 1 },
            });
            arraysToSolve.Add(new int[,]
            {
                { 1, 1 },
                { 1, 1 },
            });

            return arraysToSolve;
        }

        /// <summary>
        /// loops through array, trying to find longest diagonal as each point.
        /// uses diagonal as bounding box to sweep back to point of origin, checking each value in the box
        /// </summary>
        /// <param name="array"></param>
        /// <returns>the solution for the 2d array as a tuple of largest box origin Coordinate and length</returns>
        static (Coord, int) Solve(int[,] array)
        {
            //all non-empty arrays will have a minimum solution of a single point with square size of 1 (e.g. [[0]])
            Coord largetSquareOrigin = new Coord()
            {
                Row = 0,
                Col = 0
            };
            int largestSquareLength = 1;

            //loop through each item in array
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    //mark current position in array + current value at position (either 0 or 1)
                    int cVal = array[i, j];
                    var cCoord = new Coord()
                    {
                        Row = i,
                        Col = j
                    };
                    elementsChecked++;

                    var diagCoord = getLongestDiag(array, cCoord);

                    //found a diagonal of len > 0
                    while (diagCoord.Row > cCoord.Row && diagCoord.Col > cCoord.Row)
                    {
                        //assume we have a square contained in the bounds of the diagonal we just computed until proven otherwise
                        bool haveASquare = CheckCurrentSquare(array, cCoord, diagCoord, cVal);

                        //we checked every value in the potential square and they are all equal
                        if (haveASquare)
                        {
                            int cSquareLen = diagCoord.Row - cCoord.Row + 1;

                            //make sure the square we found isnt smaller than one we found already
                            if (cSquareLen > largestSquareLength)
                            {
                                largestSquareLength = cSquareLen;
                                largetSquareOrigin = cCoord;
                            }

                            break;
                        }
                        else
                        {
                            //we may still have a valid square along this diaglonal, but the current diagonal may be too long
                            diagCoord.Row--;
                            diagCoord.Col--;
                        }
                    }
                }
            }

            return (largetSquareOrigin, largestSquareLength);
        }

        /// <summary>
        /// checks all values in a square bounded the the NW (cCoord) and SE (diagCoord) corners
        /// </summary>
        /// <param name="array"></param>
        /// <param name="cCoord">NW corner</param>
        /// <param name="diagCoord">SE corner</param>
        /// <param name="cVal">value to compare against</param>
        /// <returns>true if all values in the square equal cVal, false otherwise</returns>
        private static bool CheckCurrentSquare(int[,] array, Coord cCoord, Coord diagCoord, int cVal)
        {
            bool haveASquare = true;
            //start at the SouthEast corner of our potential square (i.e. position indicated by the computed diagonal),
            //sweep back to cCoord (from SouthEast to NorthWest)
            for (int r = diagCoord.Row; r >= cCoord.Row; r--)
            {
                for (int c = diagCoord.Col; c >= cCoord.Col; c--)
                {
                    elementsChecked++;
                    //Make sure every value in potential square is equal
                    if (array[r, c] != cVal)
                    {
                        //values aren't equal! This is NOT a potential solution so stop the loop.
                        haveASquare = false;
                        break;
                    }
                }

                //stop the loop because we dont have a valid square
                if (!haveASquare)
                {
                    break;
                }
            }

            return haveASquare;
        }


        /// <summary>
        /// iterate from given point NW to SE, stopping when value at current position is not equal to value at given position
        /// </summary>
        /// <param name="array">the whole array</param>
        /// <param name="pos">current position</param>
        /// <returns>coordinate of the end of the diagonal (SouthEast corner of a square)</returns>
        static Coord getLongestDiag(int[,] array, Coord pos)
        {
            //coordinate to return. Initially equal to current position
            Coord coord = new Coord()
            {
                Row = pos.Row,
                Col = pos.Col
            };

            if (array != null)
            {
                //start moving from given postion to the bottom right of array
                for (int n = 1; n < int.MaxValue; n++)
                {
                    //make sure diagonal is not out of array bounds
                    if (pos.Row + n < array.GetLength(0) && pos.Col + n < array.GetLength(1))
                    {
                        elementsChecked++;
                        //update coordinate of current diagonal if the current value is equal to the value at the inital position
                        if (array[pos.Row + n, pos.Col + n] == array[pos.Row, pos.Col])
                        {
                            coord = new Coord()
                            {
                                Row = pos.Row + n,
                                Col = pos.Col + n
                            };
                        }
                        //stop the loop when we find a value that doesn't equal the current value
                        else
                        {
                            break;
                        }
                    }
                    //stop the loop when we go out of bounds
                    else
                    {
                        break;
                    }
                }
            }

            return coord;
        }

        /// <summary>
        /// utility method to print the given array in a way that looks nice :D
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        static string PrintArray(int[,] arr)
        {
            StringBuilder ret = new StringBuilder();
            ret.AppendLine(arr.Length == 0 ? "[[]]" : "[");
            if (ret.Equals("[[]]"))
            {
                return ret.ToString();
            }

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                ret.Append(" [");

                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    ret.Append(arr[i, j] + (j == arr.GetLength(1) - 1 ? string.Empty : ", "));
                }

                if (i == arr.GetLength(0) - 1)
                {
                    ret.AppendLine("]");
                }
                else
                {
                    ret.AppendLine("],");
                }
            }

            ret.Append("]");

            return ret.ToString();
        }
    }


    /// <summary>
    /// represents a position in a 2d array
    /// </summary>
    public class Coord
    {
        public int Row { get; set; }
        public int Col { get; set; }
    }
}
