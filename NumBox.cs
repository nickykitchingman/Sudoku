using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Sudoku
{
    public class NumBox
    {
        private int number;
        private int maxSize;
        private List<int> potentialNumbers;

        public SelectMode SelectMode = SelectMode.NotSelected;
        public Image image { get => button == null ? null : button.image; }

        public ButtonContainer button;
        public bool Selectable;


        public IList<int> PotentialNumbers { get => potentialNumbers; }
        //Only accept within boundary
        public int Number { get => number; set { number = value > 0 && value <= maxSize ? value : 0; } }

        public int X { get; }
        public int Y { get; }

        public NumBox(int maxSize, int x, int y)
        {
            Number = 0;
            this.maxSize = maxSize;
            X = x;
            Y = y;
            potentialNumbers = new List<int>();
            Selectable = true;
        }

        public NumBox(int maxSize, int x, int y, int number)
        {
            Number = number;
            this.maxSize = maxSize;
            X = x;
            Y = y;
            potentialNumbers = new List<int>();
            Selectable = true;
        }

        public void UpdateVisual()
        {
            //Number
            button.number.Dispatcher.Invoke(() =>{
                button.number.Text = number > 0 ? number.ToString() : "";
            });
            image.Dispatcher.Invoke(() =>
            {
                if (!Selectable && image.Opacity == SelectOpacities.NotSelected)
                    image.Opacity = SelectOpacities.UnSelectable;
            });

            //Potential numbers
            for (int i = 0; i < maxSize; i++)
            {
                TextBlock num = button.potentials[i];
                num.Dispatcher.Invoke(() =>
                {
                    if (potentialNumbers.Contains(i))
                        num.Text = i.ToString();
                    else
                        num.Text = "";
                });
            }
        }

        public void AddPotential(int num)
        {
            if (!potentialNumbers.Contains(num))
                potentialNumbers.Add(num);
        }

        public void RemovePotential(int num)
        {
            if (!potentialNumbers.Contains(num))
                potentialNumbers.Remove(num);
        }

        public void ClearPotentials()
        {
            potentialNumbers.Clear();
        }
    }
}
