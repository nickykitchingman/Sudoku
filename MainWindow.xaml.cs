using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Xml.Schema;

namespace Sudoku
{    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NumBox[,] board;
        private NumBox selected;
        private int numberStored;
        private Task solveThread;

        private int size = 3;


        public MainWindow()
        {
            InitializeComponent();
            board = Setup.InitBoard(this, size, mainGrid, myCanvas);
        }
        

        public void Image_highlight(object sender, EventArgs e)
        {
            Grid button = (Grid)sender;
            int[] coords = button.Tag.ToString().Split(' ').Select(f => Convert.ToInt32(f)).ToArray();
            NumBox numBox = board[coords[0], coords[1]];

            //Highlight if not selected, otherwise leave it
            if (numBox.Selectable)
            {
                if (numBox.SelectMode == SelectMode.NotSelected)
                    numBox.image.Opacity = SelectOpacities.HighLight;
                else if (numBox.SelectMode == SelectMode.LeftClick)
                    numBox.image.Opacity = SelectOpacities.LeftClick;
                else if (numBox.SelectMode == SelectMode.RightClick)
                    numBox.image.Opacity = SelectOpacities.RightClick;
            }
        }

        public void Image_reset(object sender, EventArgs e)
        {
            Grid button = (Grid)sender;
            int[] coords = button.Tag.ToString().Split(' ').Select(f => Convert.ToInt32(f)).ToArray();
            NumBox numBox = board[coords[0], coords[1]];

            //Leave it as it was
            if (numBox.Selectable)
            {
                if (numBox.SelectMode == SelectMode.NotSelected)
                    numBox.image.Opacity = SelectOpacities.NotSelected;
                else if (numBox.SelectMode == SelectMode.LeftClick)
                    numBox.image.Opacity = SelectOpacities.LeftClick;
                else if (numBox.SelectMode == SelectMode.RightClick)
                    numBox.image.Opacity = SelectOpacities.RightClick;
            }
        }

        public void num_leftClick(object sender, EventArgs e)
        {
            Grid button = (Grid)sender;
            click_handler(button, SelectMode.LeftClick);
        }

        public void num_rightClick(object sender, EventArgs e)
        {
            Grid button = (Grid)sender;
            click_handler(button, SelectMode.RightClick);
        }

        private void click_handler(Grid button, SelectMode selectHandle)
        {
            //Get coordinates from tag
            int[] coords = button.Tag.ToString().Split(' ').Select(f => Convert.ToInt32(f)).ToArray();

            //Select corresponding box
            NumBox numBox = board[coords[0], coords[1]];
            SelectMode selectMode = numBox.SelectMode;
            bool deselect = false;

            if (numBox.Selectable)
            {
                if (selectMode != selectHandle)
                {
                    //Opacity
                    if (selectHandle == SelectMode.LeftClick)
                        numBox.image.Opacity = SelectOpacities.LeftClick;
                    else if (selectHandle == SelectMode.RightClick)
                        numBox.image.Opacity = SelectOpacities.RightClick;

                    //Change mode
                    numBox.SelectMode = selectHandle;
                }
                else
                {
                    //Change mode and opacity
                    numBox.image.Opacity = SelectOpacities.HighLight;
                    numBox.SelectMode = SelectMode.NotSelected;
                    deselect = true;
                }

                //Deselect old box
                if (selected != null && selected != numBox)
                {
                    selected.image.Opacity = SelectOpacities.NotSelected;
                    selected.SelectMode = SelectMode.NotSelected;
                }
                selected = !deselect ? numBox : null;
            }
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Regex.IsMatch(e.Key.ToString(), @"(D\d)|(Back)|(Return)"))
            {
                //As above
                e.Handled = true;
                return;
            }

            //Backspace
            if (e.Key == Key.Back)
            {
                //Delete last digit
                string num = numberStored.ToString();
                num = num.Substring(0, num.Length - 1);
                if (num.Length > 0)
                    numberStored = int.Parse(num);
                else
                    numberStored = 0;

                //Display
                if (numberStored != 0)
                    EnterNumber.Text = numberStored.ToString();
                else
                    EnterNumber.Text = "";

                return;
            }

            //Enter number
            if (e.Key == Key.Return)
            {
                //Update button number
                if (selected != null)
                {
                    if (selected.SelectMode == SelectMode.LeftClick)
                    {
                        if (Playing.CheckValid(board, selected.X, selected.Y, numberStored, true))
                        {
                            //selected.Selectable = false;
                            selected.Number = numberStored;

                            //Clear potentials
                            selected.ClearPotentials();
                        }
                        else
                        {
                            //selected.Number = 0;
                            //selected.UpdateVisual();
                        }

                        //Reset stored number
                        numberStored = 0;
                        EnterNumber.Text = "";
                    }
                    else if (selected.SelectMode == SelectMode.RightClick)
                    {
                        selected.AddPotential(numberStored);
                    }

                    selected.UpdateVisual();
                }
                return;
            }

            //New key pressed
            int newKey = int.Parse(e.Key.ToString().Substring(1));
            if (checkValid(newKey))
            {
                //Append digit to stored number
                string num = newKey.ToString();
                if (numberStored != 0)
                    num  = numberStored.ToString() + newKey.ToString();

                //Change number stored and displayed
                if (checkValid(int.Parse(num)))
                {
                    EnterNumber.Text = num;
                    numberStored = int.Parse(num);
                }
                else
                {
                    EnterNumber.Text = newKey.ToString();
                    numberStored = newKey;
                }
            }

            bool checkValid(int num)
            {
                return num > 0 && num <= size * size;
            }
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            //Abort solving thread
            //if (solveThread != null)
            //    solveThread.();
            //Disable solve button
            ((Button)sender).IsEnabled = false;
            //Solve board visually
            solveThread = Task.Run(meth);

            void meth()
            {
                BackTrack.SetSpeedStep(10, 1.1f);
                Console.WriteLine(BackTrack.SolveRandom(board));
                BackTrack.endDisp = true;
                Setup.UpdateBoardVisual(board);
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        { 
            //Abort solving thread
            //if (solveThread != null)
            //    solveThread.Abort();
            //Reset
            Setup.ClearBoard(board);
            Setup.InitSmart(board);
            Setup.UpdateBoardVisual(board);
            //Enable solve button
            SolveButton.IsEnabled = true;
        }
    }
}

