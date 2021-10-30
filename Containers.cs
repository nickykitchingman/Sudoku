using System.Windows.Controls;

namespace Sudoku
{
    public class SectionContainer
    {
        public Grid grid;
        public Border border;
        public Viewbox viewbox;
        public TextBlock number;
        public TextBlock[] potentials;

        public SectionContainer(Grid grid, Border border, Viewbox viewbox)
        {
            this.grid = grid;
            this.border = border;
            this.viewbox = viewbox;
        }

        public SectionContainer(Grid grid, Viewbox viewbox, TextBlock number, TextBlock[] potentials)
        {
            this.grid = grid;
            this.viewbox = viewbox;
            this.number = number;
            this.potentials = potentials;
        }
    }

    public class ButtonContainer
    {
        public Grid grid;
        public Image image;
        public TextBlock number;
        public TextBlock[] potentials;

        public ButtonContainer(Grid grid, Image image, TextBlock number, TextBlock[] potentials)
        {
            this.grid = grid;
            this.image = image;
            this.potentials = potentials;
            this.number = number;
        }
    }
}
