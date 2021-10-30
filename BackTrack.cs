using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Sudoku
{
    class BackTrack
    {
        private class Coord
        {
            public int x;
            public int y;

            public Coord()
            {
                x = 0; y = 0;
            }
            
            public Coord(int x, int y)
            {
                this.x = x; this.y = y;
            }
        }

        private static NumBox _current;

        private static NumBox GetCurrent()
        {
            return _current;
        }

        private static void SetCurrent(NumBox value)
        {
            if (_current != null)
                _current.image.Dispatcher.Invoke(() =>
                {
                    _current.image.Opacity = 1;
                    _current = value;
                    if (value != null)
                        _current.image.Opacity = 0.5;
                });
            else
                _current = value;
        }

        public static bool Solve(NumBox[,] board)
        {
            //Find next empty box
            NumBox empty = null;
            foreach (NumBox box in board)
                if (box.Number == 0)
                {
                    empty = box;
                    break;
                }

            //Solved
            if (empty == null)
                return true;


            //Backtrack
            int sqrSize = board.GetLength(0);
            for (int num = 1; num <= sqrSize; num++)
            {
                if (Playing.CheckValid(board, empty.X, empty.Y, num))
                {
                    empty.Number = num;
                    //Display step
                    displayStep(empty);

                    if (Solve(board))
                        return true;
                    else
                    {
                        empty.Number = 0;
                        //Display step
                        displayStep(empty);
                    }
                }
            }

            //Unsolvable
            return false;
        }

        
        public static bool SolveRandom(NumBox[,] board)
        {
            int sqrSize = board.GetLength(0);

            //Find next empty box
            List<NumBox> empties = new List<NumBox>();
            foreach (NumBox box in board)
                if (box.Number == 0)
                    empties.Add(box);

            //Solved
            if (empties.Count <= 0)
                return true;

            //Random empty
            Random rand = new Random();
            NumBox nextBox = empties[rand.Next(empties.Count)];
            Console.WriteLine(empties.IndexOf(nextBox));

            //Backtrack
            for (int num = 1; num <= sqrSize; num++)
            {
                if (Playing.CheckValid(board, nextBox.X, nextBox.Y, num))
                {
                    nextBox.Number = num;
                    //Display step
                    displayStep(nextBox);

                    if (SolveRandom(board))
                        return true;
                    else
                    {
                        nextBox.Number = 0;
                        //Display step
                        displayStep(nextBox);
                    }
                }
            }

            //Unsolvable
            return false;
        }

        public static bool SolveRandom2(NumBox[,] board)
        {
            //Find next empty box
            NumBox empty = null;
            foreach (NumBox box in board)
                if (box.Number == 0)
                {
                    empty = box;
                    break;
                }

            //Solved
            if (empty == null)
                return true;


            //Backtrack
            int sqrSize = board.GetLength(0);
            //Randomise number order
            Random rand = new Random();
            int[] nums = Enumerable.Range(1, 9).OrderBy(f => rand.Next()).ToArray();
            foreach (int num in nums)
            {
                if (Playing.CheckValid(board, empty.X, empty.Y, num))
                {
                    //Try number
                    empty.Number = num;
                    //Display step
                    displayStep(empty);

                    //Solved 
                    if (SolveRandom2(board))
                        return true;
                    else
                    {
                        //Unsolvable thread
                        empty.Number = 0;
                        //Display step
                        displayStep(empty);
                    }
                }
            }

            //Unsolvable
            return false;
        }

        public static bool SolveRandom(int[,] board)
        {
            int sqrSize = board.GetLength(0);

            //Find next empty box
            List<Coord> empties = new List<Coord>();
            for (int i = 0; i < sqrSize; i++)
                for (int j = 0; j < sqrSize; j++)
                    if (board[i, j] == 0)
                        empties.Add(new Coord(i, j));

            //Solved
            if (empties.Count <= 0)
                return true;

            //Random empty
            Random rand = new Random();
            Coord nextBox = empties[rand.Next(empties.Count)];

            //Backtrack
            for (int num = 1; num <= sqrSize; num++)
            {
                if (Playing.CheckValid(board, nextBox.x, nextBox.y, num))
                {
                    board[nextBox.x, nextBox.y] = num;
                    if (SolveRandom(board))
                        return true;
                    else
                        board[nextBox.x, nextBox.y] = 0;
                }
            }

            //Unsolvable
            return false;
        }

        public static bool Solve(int[,] board)
        {
            int sqrSize = board.GetLength(0);

            //Find next empty box
            Coord box = null;
            for (int i = 0; i < sqrSize; i++)
            {
                for (int j = 0; j < sqrSize; j++)
                    if (board[i, j] == 0)
                    {
                        box = new Coord(i, j);
                        goto End;
                    }
            }
        End:

            //Solved
            if (box == null)
                return true;


            //Backtrack
            for (int num = 1; num <= sqrSize; num++)
            {
                if (Playing.CheckValid(board, box.x, box.y, num))
                {
                    board[box.x, box.y] = num;
                    if (Solve(board))
                        return true;
                    else
                        board[box.x, box.y] = 0;
                }
            }

            //Unsolvable
            return false;
        }

        public static bool SolveRandom2(int[,] board)
        {
            int sqrSize = board.GetLength(0);

            //Find next empty box
            Coord box = null;
            for (int i = 0; i < sqrSize; i++)
            {
                for (int j = 0; j < sqrSize; j++)
                    if (board[i, j] == 0)
                    {
                        box = new Coord(i, j);
                        goto End;
                    }
            }
        End:

            //Solved
            if (box == null)
                return true;


            //Randomise number order
            Random rand = new Random();
            int[] nums = Enumerable.Range(1, 9).OrderBy(f => rand.Next()).ToArray();
            //Backtrack
            foreach (int num in nums)
            {
                if (Playing.CheckValid(board, box.x, box.y, num))
                {
                    board[box.x, box.y] = num;
                    if (SolveRandom2(board))
                        return true;
                    else
                        board[box.x, box.y] = 0;
                }
            }

            //Unsolvable
            return false;
        }

        private static int dispSpeed = 0;
        public static bool endDisp = false;

        public static void SetSpeedStep(int speed, float increase)
        {
            dispSpeed = speed;
            int iters = 0;

            //Start new thread
            Task t = Task.Run(updateTime);

            void updateTime()
            {
                while (!endDisp && iters++ < 10000 && speed > 0)
                {
                    Thread.Sleep(1000);
                    dispSpeed = (int)(dispSpeed / increase);
                }
                endDisp = false;
            }
        }

        private static void displayStep(NumBox control)
        {
            //Wait, then change opacity
            if (dispSpeed > 0)
            {
                Thread.Sleep(dispSpeed);
                SetCurrent(control);
                control.image.Dispatcher.Invoke(() =>
                {
                    control.UpdateVisual();
                });
            }
        }
    }
}
