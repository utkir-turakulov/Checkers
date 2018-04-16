using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Сheckers
{
    public class Checker : IShape
    {
        private Border _border;
        private Color _color;
        private Ellipse _ellipse;
        private Border[,] _cells;
        private Cell cell ;
        private readonly int _queenSide; // параметр отвечающий за строку в которой шашка становится дамкой 



        protected enum ShapeColor : int
        {
            Black = 1,
            White = 2
        }

        private enum Type
        {
            Queen,
            Checker
        }


        public Checker(Border border, Color color)
        {
            this._border = border;
            _color = color;
            cell = new Cell
            {
                Row = Grid.GetRow(border),
                Coll = Grid.GetColumn(border)
            };

            if (color == Brushes.White.Color)
            {
                _ellipse = new Ellipse// Здесь продумать как переделать заполнение вместо Ellipse сделать Checker или Queen
                {
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Fill = Brushes.White
                };
                _queenSide = 0;
            }
            if (color == Brushes.Black.Color)
            {
                _ellipse = new Ellipse
                {
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Fill = Brushes.Black
                };
                _queenSide = 7;
            } 
        }
        public int QueenSide()
        {
            return _queenSide;
        }

        public Cell GetCell()
        {
            return cell;
        }

        public Ellipse GetEllipse()
        {
            return _ellipse;
        }

        public  void SetColor(Color col)
        {
            _color = col;
        }

        public void SetBorder(Border border)
        {
            this._border = border;
        }

        public  Color GetColor()
        {
            return this._color;
        }

        public  bool Move(Border border, Border[,] cells)
        {
            Ellipse ellipse = (Ellipse)border.Child;
            this._cells = cells;
            int oldRow = cell.Row;//Grid.GetRow(_border);
            int oldCol = cell.Coll;//Grid.GetColumn(_border);
            int newCol = Grid.GetColumn(border);
            int newRow = Grid.GetRow(border);

      
            if(ellipse == null)
            {
                if (IsCorrectStep(oldRow, oldCol, newRow, newCol))
                {
                    _cells[oldRow, oldCol].Child = null;
                    _cells[Grid.GetRow(border), Grid.GetColumn(border)].Child = _ellipse;
                    return true;
                }

                if (oldRow - 2 == newRow && oldCol - 2 == newCol)//верх newRow - oldCol == 2 && newCol - oldCol == 2 || newRow + 2 == oldRow && newCol - 2 == oldCol
                {
                    if (MoveAndDelete(oldRow - 1, oldCol - 1, oldRow, oldCol, border))
                    {
                        return true;
                    }
                    else
                        return false;
                }

                if (oldRow - 2 == newRow && oldCol + 2 == newCol)
                {
                    if(MoveAndDelete(oldRow - 1, oldCol + 1, oldRow, oldCol, border))
                    {
                        return true;
                    }
                    else
                        return false;
                }

                if (oldRow + 2 == newRow && oldCol - 2 == newCol)//низ
                {
                    if(MoveAndDelete(oldRow + 1, oldCol - 1, oldRow, oldCol, border))
                    {
                        return true;
                    }
                    else
                        return false;
                }

                if (oldRow + 2 == newRow && oldCol + 2 == newCol)
                {
                    if(MoveAndDelete(oldRow + 1, oldCol + 1, oldRow, oldCol, border))
                    {
                        return true;
                    }
                    else
                        return false;

                }
                
            }
            return false;

        }//Move()


        public bool MoveAndDelete(int enemyRow, int enemyCol, int oldRow, int oldCol, Border border)
        {
            Border enemy = _cells[enemyRow, enemyCol];
            Ellipse enemyEllips = (Ellipse)enemy.Child;
            if (enemyEllips != null)
            {
                if (enemyEllips.Fill != _ellipse.Fill)
                {
                    if (enemyEllips.Fill == Brushes.Black)
                    {
                        MessageBox.Show("Бьем черну фигуру");
                    }

                    else
                    {
                        MessageBox.Show("Бьем белую фигуру");
                    }
                    _cells[enemyRow, enemyCol].Child = null;//удаляем противника из доски
                    _cells[oldRow, oldCol].Child = null;//удаляем свою фишку
                    _cells[Grid.GetRow(border), Grid.GetColumn(border)].Child = _ellipse;//переносим свою фишку
                    return true;
                }
            }
            return false;
        }//MoveAndDelete()

        public bool IsCorrectStep(int oldRow, int oldCol, int newRow, int newCol)
        {
            bool isCorrect = false;
            if (oldRow - 1 == newRow && oldCol - 1 == newCol || oldRow - 1 == newRow && oldCol + 1 == newCol)//верх
                isCorrect = true;
            if (oldRow + 1 == newRow && oldCol - 1 == newCol || oldRow + 1 == newRow && oldCol + 1 == newCol)//низ
                isCorrect = true;
            return isCorrect;
        }//IsCorrectStep

        
    }
}
