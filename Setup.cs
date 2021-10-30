using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sudoku
{
    public static class Setup
    {
        private static NumBox[,] board;
        private static int size;
        private static double minLength;
        private static BitmapImage normalBitmap;
        private static Canvas myCanvas;
        private static MainWindow window;


        private static void initNumbers()
        {
            //int[,] copy = null;
            //do
            //{
            Restart:
                board.Initialize();

                //Generate random numbers 
                int total = 19;
                byte[] buffer = new byte[total];
                int[] nums;

                Random rand = new Random();
                rand.NextBytes(buffer);
                nums = buffer.Select(f => (int)((float)f / 255 * size * size) + 1).ToArray();

                int count = 1;
                foreach (int num in nums)
                {
                    //Choose random coord
                    int x, y, iter = 0;
                    bool valid;
                    do
                    {
                        x = rand.Next(size * size);
                        y = rand.Next(size * size);
                        valid = Playing.CheckValid(board, x, y, num);
                        if (iter++ > nums.Length * 3)
                        {
                            ClearBoard(board);
                            goto Restart;
                        }
                        Console.WriteLine(num);
                    } while (board[x, y].Number != 0 || !valid);

                    //Set random number
                    board[x, y].Number = num;
                    board[x, y].Selectable = false;
                    Console.WriteLine(count++);
                }

            //    //Solve copy
            //    copy = new int[size * size, size * size];
            //    foreach (NumBox box in board)
            //        copy[box.X, box.Y] = box.Number;
            //} while (!BackTrack.Solve(copy));
        }

        public static void InitSmart(NumBox[,] board, int total = 20)
        {
            Setup.board = board;
            size = (int)Math.Sqrt(board.GetLength(0));
            populateBoard();
            //Solve a random copy
            int[,] copy;
            do
            { 
                copy = new int[size * size, size * size];
                foreach (NumBox box in board)
                    copy[box.X, box.Y] = box.Number;

            } while(!BackTrack.SolveRandom2(copy));


            //Generate random coords 
            Random rand = new Random();

            //Select some numbers from solution
            for (int count = 0; count < total; count++) 
            {
                //Choose random coord
                int x, y;
                do
                {
                    x = rand.Next(size * size);
                    y = rand.Next(size * size);
                } while (board[x, y].Number != 0);

                //Set random number
                board[x, y].Number = copy[x, y];
                board[x, y].Selectable = false;
            }
        }

        private static void populateBoard()
        {
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    board[i, j] = new NumBox(size * size, i, j);
        }

        public static void ClearBoard(NumBox[,] board)
        {
            populateBoard();
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    board[i, j].Number = 0;
                    board[i, j].Selectable = true;
                    if (board[i,j].image != null)
                        board[i, j].image.Opacity = SelectOpacities.NotSelected;
                }
        }

        public static void UpdateBoardVisual(NumBox[,] board)
        {
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    if (board[i, j] != null)
                        if (board[i, j].image != null)
                            board[i, j].UpdateVisual();
        }

        public static NumBox[,] InitBoard(MainWindow mainWindow, int boardSize, Grid mainGrid, Canvas canvas)
        {
            //Initiate actual board 
            window = mainWindow;
            size = boardSize;
            board = new NumBox[size * size, size * size];
            populateBoard();
            myCanvas = canvas;
            InitSmart(board);
            //initNumbers();
            minLength = Math.Min(myCanvas.Width, myCanvas.Height);

            //Set grid
            mainGrid.Background = Brushes.Black;

            //Initialise some variables
            Uri uri = new Uri("Square.png", UriKind.Relative);
            normalBitmap = new BitmapImage(uri);

            //Add rows + columns based on size var
            for (int i = 0; i < size; i++)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition());
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            //Add every section
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    SectionContainer section = createSection(x, y, true);
                    mainGrid.Children.Add(section.viewbox);
                }

            //Update visual board
            UpdateBoardVisual(board);
            return board;
        }

        /// <summary>
        /// type: true = buttons, false = labels
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static SectionContainer createSection(int i, int j, bool type)
        {
            Grid section = new Grid
            {
                ClipToBounds = true,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Border border = null;
            if (type)
                border = new Border
                {
                    Child = section,
                    Width = minLength / size,
                    BorderThickness = new Thickness(2),
                    BorderBrush = Brushes.Black,
                    Background = Brushes.Black
                };

            Grid canvas = new Grid();
            Viewbox viewbox = new Viewbox();
            viewbox.Child = canvas;
            if (type)
            {
                viewbox.Width = viewbox.Height = minLength / size;
                //viewbox.Child = border;
                canvas.Children.Add(border);
            }
            else
            {
                viewbox.Width = viewbox.Height = minLength / (size * size);
                //viewbox.Child = section;
                canvas.Children.Add(section);
            }

            //Position of section in board
            int startX = i * size;
            int startY = j * size;

            //Add rows + columns of given size
            for (int p = 0; p < size; p++)
            {
                section.RowDefinitions.Add(new RowDefinition());
                section.ColumnDefinitions.Add(new ColumnDefinition());
            }

            //Set section position
            Grid.SetRow(viewbox, i);
            Grid.SetColumn(viewbox, j);

            //Middle number
            TextBlock number = null;
            if (!type)
            {
                number = new TextBlock
                {
                    Text = "1",
                    FontSize = 30,
                    FontWeight = FontWeights.SemiBold,
                    Opacity = 0.8,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                //Panel.SetZIndex(label, 10);
                canvas.Children.Add(number);
            }

            //Add buttons or labels to every row + column
            TextBlock[] potentials = new TextBlock[size * size];
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    double width = minLength / (size * size);
                    double height = minLength / (size * size);
                    if (type)
                    {
                        //Buttons
                        ButtonContainer button = createNumButton(startX + x, startY + y, width, height);
                        board[startX + x, startY + y].button = button;
                        Grid.SetRow(button.grid, x);
                        Grid.SetColumn(button.grid, y);

                        section.Children.Add(button.grid);
                    }
                    else
                    {
                        //Labels in buttons
                        TextBlock potential = new TextBlock()
                        {
                            Text = "",
                            FontSize = 10,
                            Opacity = 0.5,
                            FontWeight = FontWeights.Heavy,
                            Foreground = Brushes.Gray,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        Grid.SetRow(potential, x);
                        Grid.SetColumn(potential, y);

                        potentials[x * size + y] = potential;
                        section.Children.Add(potential);
                    }
                }

            if (type)
                return new SectionContainer(section, border, viewbox);
            else
                return new SectionContainer(section, viewbox, number, potentials);
        }

        private static ButtonContainer createNumButton(int x, int y, double width, double height)
        {
            //Image
            Image image = new Image()
            {
                Source = normalBitmap,
                Stretch = Stretch.UniformToFill,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                MaxHeight = width,
                MaxWidth = height,
                Tag = x + " " + y
            };

            //Container
            Grid grid = new Grid()
            {
                Tag = x + " " + y
            };
            grid.Children.Add(image);

            //Act like a button
            grid.MouseEnter += window.Image_highlight;
            grid.MouseLeave += window.Image_reset;
            grid.MouseLeftButtonDown += window.num_leftClick;
            grid.MouseRightButtonDown += window.num_rightClick;

            //Labels
            SectionContainer labels = createSection(0, 0, false);
            grid.Children.Add(labels.viewbox);

            return new ButtonContainer(grid, image, labels.number, labels.potentials);
        }
    }
}
