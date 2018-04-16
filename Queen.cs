using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Сheckers
{
    class Queen : IShape
    {

        private Border _border;
        private Color _color;
        private Ellipse _ellipse;
        private Border[,] _cells;
        private Cell cell;
        private readonly int _queenSide;


        public Queen(Border border, Color color)
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
                    Fill = Brushes.White,
                    Stroke = Brushes.Black,
                    StrokeThickness = 5

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
                    Fill = Brushes.Black,
                    Stroke = Brushes.White,
                    StrokeThickness = 5
                };
                _queenSide = 7;

            }
        }


        public Cell GetCell()
        {
            return cell;
        }

        public Color GetColor()
        {
            return _color;
        }
        public void SetColor(Color color)
        {
            _color = color;
        }

        public bool Move(Border border, Border[,] cells)
        {
            Ellipse ellipse = (Ellipse)border.Child;


            this._cells = cells;
            int oldRow = cell.Row;//Grid.GetRow(_border);
            int oldCol = cell.Coll;//Grid.GetColumn(_border);
            int newCol = Grid.GetColumn(border);
            int newRow = Grid.GetRow(border);


            if (ellipse == null)
            {
                if (IsCorrectStep(oldRow, oldCol, newRow, newCol))
                {
                    _cells[oldRow, oldCol].Child = null;
                    _cells[Grid.GetRow(border), Grid.GetColumn(border)].Child = _ellipse;
                    return true;
                }

                if (oldRow - 2 == newRow && oldCol - 2 == newCol)
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
                    if (MoveAndDelete(oldRow - 1, oldCol + 1, oldRow, oldCol, border))
                    {
                        return true;
                    }
                    else
                        return false;
                }

                if (oldRow + 2 == newRow && oldCol - 2 == newCol)//низ
                {
                    if (MoveAndDelete(oldRow + 1, oldCol - 1, oldRow, oldCol, border))
                    {
                        return true;
                    }
                    else
                        return false;
                }

                if (oldRow + 2 == newRow && oldCol + 2 == newCol)
                {
                    if (MoveAndDelete(oldRow + 1, oldCol + 1, oldRow, oldCol, border))
                    {
                        return true;
                    }
                    else
                        return false;

                }

                if (oldRow == newRow && oldCol == newCol)
                {
                    cells[oldRow, oldCol].Child = null;
                    _cells[newRow, newCol].Child = _ellipse;
                }

                if (BigStep(oldRow, oldCol, newRow, newCol, border))
                {
                    cells[oldRow, oldCol].Child = null;
                    _cells[newRow, newCol].Child = _ellipse;
                    return true;
                }
            }
            return false;

        }
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

        public bool BigStep(int enemyRow, int enemyCol, int oldRow, int oldCol, Border border)//Шаг дамки
        {
            int potentialRow = 0;
            int potentialCol = 0;
            int enemyCounter = 0;
            Border enemyBorder = new Border();
            List<Ellipse> list = new List<Ellipse>();
            List<Dictionary<string,int>> cells = new List<Dictionary<string, int>>();
            Dictionary <string, int> dictionary;
            if (oldRow > enemyRow && oldCol < enemyCol)
            {
                while (oldRow > enemyRow && oldCol < enemyCol)
                {
                    enemyRow++;
                    enemyCol--;
                    if (enemyCounter != 2)
                    {
                        if (IsEnemy(_cells[enemyRow, enemyCol]) && !IsAlly(_cells[enemyRow, enemyCol]))
                        {
                            potentialCol = enemyCol;
                            potentialRow = enemyRow;
                            enemyCounter++;
                            list.Add(_cells[enemyRow, enemyCol].Child as Ellipse);
                            dictionary = new Dictionary<string, int>
                            {
                                { "row", enemyRow },
                                { "col", enemyCol }
                            };
                            cells.Add(dictionary);
                        }
                        else
                        {
                            enemyCounter = 0;
                            //_cells[potentialRow, potentialCol] = null;
                        }
                    }
                    else
                    {
                        //_cells[potentialRow, potentialCol].Child = enemyBorder.Child;
                        return false;
                    }

                }
                for (int i = 0; i < list.Count; i++)
                {
                    _cells[cells[i]["row"], cells[i]["col"]].Child = null;
                }

                return true;
            }
            if (oldRow < enemyRow && oldCol > enemyCol)//тестовая ветвь
            {
                while (oldRow < enemyRow && oldCol > enemyCol)
                {
                    enemyRow--;
                    enemyCol++;
                    if (enemyCounter != 2)
                    {
                        if (IsEnemy(_cells[enemyRow, enemyCol]) && !IsAlly(_cells[enemyRow, enemyCol]))
                        {
                            potentialCol = enemyCol;
                            potentialRow = enemyRow;
                            enemyCounter++;
                            list.Add(_cells[enemyRow, enemyCol].Child as Ellipse);
                            dictionary = new Dictionary<string, int>
                            {
                                { "row", enemyRow },
                                { "col", enemyCol }
                            };
                            cells.Add(dictionary);
                        }
                        else
                        {
                            enemyCounter = 0;
                            //_cells[potentialRow, potentialCol] = null;
                        }
                    }
                    else
                    {
                        //_cells[potentialRow, potentialCol].Child = enemyBorder.Child;
                        return false;
                    }

                }
                for(int i=0; i< list.Count; i++)
                {
                    _cells[cells[i]["row"], cells[i]["col"]].Child = null;
                }

                return true;
            }
            if (oldRow < enemyRow && oldCol < enemyCol)
            {
                while (oldRow < enemyRow && oldCol < enemyCol)
                {
                    enemyRow--;
                    enemyCol--;
                    if (enemyCounter != 2)
                    {
                        if (IsEnemy(_cells[enemyRow, enemyCol]) && !IsAlly(_cells[enemyRow, enemyCol]))
                        {
                            potentialCol = enemyCol;
                            potentialRow = enemyRow;
                            enemyCounter++;
                            list.Add(_cells[enemyRow, enemyCol].Child as Ellipse);
                            dictionary = new Dictionary<string, int>
                            {
                                { "row", enemyRow },
                                { "col", enemyCol }
                            };
                            cells.Add(dictionary);
                        }
                        else
                        {
                            enemyCounter = 0;
                            //_cells[potentialRow, potentialCol] = null;
                        }
                    }
                    else
                    {
                        //_cells[potentialRow, potentialCol].Child = enemyBorder.Child;
                        return false;
                    }

                }
                for (int i = 0; i < list.Count; i++)
                {
                    _cells[cells[i]["row"], cells[i]["col"]].Child = null;
                }

                return true;
            }
            if (oldRow > enemyRow && oldCol > enemyCol)
            {
                while (oldRow > enemyRow && oldCol > enemyCol)
                {
                    enemyRow++;
                    enemyCol++;
                    if (enemyCounter != 2)
                    {
                        if (IsEnemy(_cells[enemyRow, enemyCol]) && !IsAlly(_cells[enemyRow, enemyCol]))
                        {
                            potentialCol = enemyCol;
                            potentialRow = enemyRow;
                            enemyCounter++;
                            list.Add(_cells[enemyRow, enemyCol].Child as Ellipse);
                            dictionary = new Dictionary<string, int>
                            {
                                { "row", enemyRow },
                                { "col", enemyCol }
                            };
                            cells.Add(dictionary);
                        }
                        else
                        {
                            enemyCounter = 0;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
                for (int i = 0; i < list.Count; i++)
                {
                    _cells[cells[i]["row"], cells[i]["col"]].Child = null;
                }

                return true;
            }

            return false;
        }


        private bool IsEnemy(Border border)
        {
            Ellipse ellipse = (Ellipse)border.Child;
            if (ellipse != null)
            {
                if (ellipse.Fill == _ellipse.Fill)
                {
                    return false;
                }
                return true;
            }

            return false;
        }

        private void TryMakeBigMove()
        {

        }

        /*Союзник*/
        private bool IsAlly(Border border)
        {
            Ellipse ellipse = (Ellipse)border.Child;
            if (ellipse != null)
            {
                if (ellipse.Fill == _ellipse.Fill)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public int QueenSide()
        {
            return _queenSide;
        }
    }
}
