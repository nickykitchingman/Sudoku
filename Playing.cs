using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sudoku
{
    class Playing
    {
        private static Task flashing;

        public static bool CheckValid(NumBox[,] board, int x, int y, int num, bool show = false)
        {
            int sqrSize = board.GetLength(0);
            int size = (int)Math.Sqrt(sqrSize);

            //Check row and column
            for (int i = 0; i < sqrSize; i++)
            {
                if (board[i, y].Number == num)
                {
                    if(show) flashInvalid(board[i, y]);
                    return false;
                }
                if (board[x, i].Number == num)
                {
                    if (show) flashInvalid(board[x, i]);
                    return false;
                }
            }

            //Check section
            int startX = x / size * size;
            int startY = y / size * size;
            for (int i = startX; i < startX + size; i++)
                for (int j = startY; j < startY + size; j++)
                {
                    if (i < sqrSize && j < sqrSize)
                        if (board[i, j].Number == num)
                        {
                            if (show) flashInvalid(board[i, j]);
                            return false;
                        }
                }

            return true;
        }

        public static bool CheckValid(int[,] board, int x, int y, int num)
        {
            int sqrSize = board.GetLength(0);
            int size = (int)Math.Sqrt(sqrSize);

            //Check row and column
            for (int i = 0; i < sqrSize; i++)
            {
                if (board[i, y] == num)
                    return false;
                if (board[x, i] == num)
                    return false;
            }

            //Check section
            int startX = x / size * size;
            int startY = y / size * size;
            for (int i = startX; i < startX + size; i++)
                for (int j = startY; j < startY + size; j++)
                {
                    if (i < sqrSize && j < sqrSize)
                        if (board[i, j] == num)
                            return false;
                }

            return true;
        }

        
        private static void flashInvalid(NumBox box, int time = 1000)
        {
            // Finish flashing
            if (flashing != null)
                if (!flashing.IsCompleted)
                    flashing.Wait();

            //Start new thread
            flashing = new Task(meth);
            flashing.Start();

            void meth()
            {
                double temp = 0;
                //Set image opacity for a time
                box.image.Dispatcher.Invoke(() =>
                {
                    temp = box.image.Opacity;
                    box.image.Opacity = 0.3;
                    box.UpdateVisual();
                });
                Thread.Sleep(time);
                box.image.Dispatcher.Invoke(() =>
                {
                    box.image.Opacity = temp;
                    box.UpdateVisual();
                });
            }
        }
    }
}
