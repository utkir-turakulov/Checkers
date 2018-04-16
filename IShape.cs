using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Сheckers
{
    public interface IShape
    {
        //private Border _border;
        //private Color _color;
        //private Cell _cell;
        //private readonly int _queenSide;


        //public IShape()
        //{

        //}

        //public IShape(Border border, Color color)
        //{
        //    _border = border;
        //    _color = color;

        //}


        //public abstract bool Move(Border border,Border [,] _cells);
        //public abstract Color GetColor();
        //public abstract void SetColor(Color color);
        //public abstract Cell GetCell();

        bool Move(Border border, Border[,] _cells);
        Color GetColor();
        void SetColor(Color color);
        Cell GetCell();
        int QueenSide();
    }
}
