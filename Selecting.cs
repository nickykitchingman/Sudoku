
namespace Sudoku
{
    public enum SelectMode
    {
        NotSelected,
        LeftClick,
        RightClick
    }

    public struct SelectOpacities
    {
        public const double NotSelected = 1;
        public const double LeftClick = 0.7;
        public const double RightClick = 0.8;
        public const double HighLight = 0.9;
        public const double UnSelectable = 0.95;
    }
}
