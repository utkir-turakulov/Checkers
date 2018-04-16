using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Сheckers
{
    class Game 
    {
       /* private readonly Border[,] _cells = new Border[8, 8];
        private readonly IShape[,] shapes = new Checker[8, 8];
        private int[,] checker_pos = new int[8, 8];
        private readonly Ellipse[] _whiteCheckers = new Ellipse[12];
        private readonly Ellipse[] _blackCheckers = new Ellipse[12];
        private int _whiteSteps = 0;
        private int[] pos = new int[2];
        private Ellipse lastClicked;*/


        private List<Step> steps;
        private List<IObserver> observers;

        private Table window;
        private CheckerTable table; 

        public Game(Table window)
        {
            this.window = window;
            observers = new List<IObserver>();
            steps = new List<Step>();
            table = new CheckerTable(window);
            NetComponent net = new NetComponent();
            table.AddObserver(net);
        }

        public void Start()
        {
            table.SetupRowsAndColumns();
            table.SetupCells();
            table.SetupCheckers();
        }
/*
        public void SetupCells()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    _cells[i, j] = new Border
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Background = (i % 2 != j % 2) ? Brushes.SaddleBrown : Brushes.DarkSalmon,
                    };
                    _cells[i, j].SetValue(Grid.RowProperty, i);
                    _cells[i, j].SetValue(Grid.ColumnProperty, j);
                    _cells[i, j].MouseLeftButtonDown += MoveChecker;//Функция нажатия на ячейку

                    window.MainGrid.Children.Add(_cells[i, j]);
                }
            }
        }

        public void SetupRowsAndColumns()
        {
            for (int i = 0; i < 8; i++)
            {
                window.MainGrid.RowDefinitions.Add(new RowDefinition());
                window.MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        public void SetupCheckers()
        {
            int iWhite = 0;
            for (int row = 5; row < 8; row++)
            {
                for (int pos = 0; pos < 8; pos += 2)
                {
                    _whiteCheckers[iWhite] = new Ellipse// Здесь продумать как переделать заполнение вместо Ellipse сделать Checker или Queen
                    {
                        Margin = new Thickness(5),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Fill = Brushes.White
                    };
                    _cells[row, pos + (row + 1) % 2].Child = _whiteCheckers[iWhite];
                    //shapes[row, pos + (row + 1) % 2] = new Checker(border);
                    checker_pos[row, pos + (row + 1) % 2] = 1;
                    iWhite++;
                }
            }

            int iBlack = 0;
            for (int row = 0; row < 3; row++)
            {
                for (int pos = 0; pos < 8; pos += 2)
                {
                    _blackCheckers[iBlack] = new Ellipse
                    {
                        Margin = new Thickness(5),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Fill = Brushes.Black
                    };
                    //  _blackCheckers[iBlack].MouseLeftButtonDown += BlackMoove; 
                    _cells[row, pos + (row + 1) % 2].Child = _blackCheckers[iBlack];
                    checker_pos[row, pos + (row + 1) % 2] = 2;
                    iBlack++;
                }
            }
        }

        public void MoveChecker(object sender, MouseButtonEventArgs e)
        {
            Border border = (Border)sender;
            Ellipse ellipse = (Ellipse)border.Child;

            if (ellipse != null)//если ячейка пуста, то переносим фишку 
            {
                _whiteSteps++;

                if (ellipse.Fill == Brushes.White)
                {
                    lastClicked = new Ellipse
                    {
                        Margin = new Thickness(5),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Fill = Brushes.White
                    };
                    Move(border);
                }

                if (ellipse.Fill == Brushes.Black)
                {
                    lastClicked = new Ellipse
                    {
                        Margin = new Thickness(5),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Fill = Brushes.Black
                    };
                    Move(border);
                }
            }
            else
            {
                _whiteSteps++;
                Move(border);
                _whiteSteps = 0;
            }

         
        }


        public void Move(Border border)
        {
            Ellipse ellipse = (Ellipse)border.Child;

            if (ellipse != null)
            {
                pos[0] = Grid.GetRow(border);//первый клик строка
                pos[1] = Grid.GetColumn(border);//первый клик столбец
            }
            else
            {
                if (_whiteSteps == 2)
                {
                    if (IsCorrectStep(pos[0], pos[1], Grid.GetRow(border), Grid.GetColumn(border)))
                    {
                        _cells[pos[0], pos[1]].Child = null;
                        checker_pos[pos[0], pos[1]] = 0;
                        _cells[Grid.GetRow(border), Grid.GetColumn(border)].Child = lastClicked;


                        if (lastClicked.Fill == Brushes.White)
                        {
                            checker_pos[Grid.GetRow(border), Grid.GetColumn(border)] = 1;
                        }
                        else if (lastClicked.Fill == Brushes.Black)
                        {
                            checker_pos[Grid.GetRow(border), Grid.GetColumn(border)] = 2;
                        }

                    }


                    int oldRow = pos[0];
                    int oldCol = pos[1];
                    int newCol = Grid.GetColumn(border);
                    int newRow = Grid.GetRow(border);

                    if (oldRow - 2 == newRow && oldCol - 2 == newCol)//верх newRow - oldCol == 2 && newCol - oldCol == 2 || newRow + 2 == oldRow && newCol - 2 == oldCol
                    {
                        MoveAndDelete(oldRow - 1, oldCol - 1, oldRow, oldCol, border);
                    }

                    if (oldRow - 2 == newRow && oldCol + 2 == newCol)
                    {
                        MoveAndDelete(oldRow - 1, oldCol + 1, oldRow, oldCol, border);
                    }

                    if (oldRow + 2 == newRow && oldCol - 2 == newCol)//низ
                    {
                        MoveAndDelete(oldRow + 1, oldCol - 1, oldRow, oldCol, border);
                    }

                    if (oldRow + 2 == newRow && oldCol + 2 == newCol)
                    {
                        MoveAndDelete(oldRow + 1, oldCol + 1, oldRow, oldCol, border);

                    }
                }
                NotifyObservers();//оповостили сеть об изменении
            }
        }
        public bool IsCorrectStep(int oldRow, int oldCol, int newRow, int newCol)
        {
            bool isCorrect = false;
            if (oldRow - 1 == newRow && oldCol - 1 == newCol || oldRow - 1 == newRow && oldCol + 1 == newCol)//верх
                isCorrect = true;
            if (oldRow + 1 == newRow && oldCol - 1 == newCol || oldRow + 1 == newRow && oldCol + 1 == newCol)//низ
                isCorrect = true;
            return isCorrect;
        }





        public void MoveAndDelete(int enemyRow, int enemyCol, int oldRow, int oldCol, Border border)
        {
            Border enemy = _cells[enemyRow, enemyCol];
            Ellipse enemyEllips = (Ellipse)enemy.Child;
            if (enemyEllips != null)
            {
                if (enemyEllips.Fill != lastClicked.Fill)
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

                    checker_pos[enemyRow, enemyCol] = 0;
                    checker_pos[oldRow, oldCol] = 0;
                    _cells[Grid.GetRow(border), Grid.GetColumn(border)].Child = lastClicked;//переносим свою фишку


                    if (lastClicked.Fill == Brushes.White)
                    {
                        checker_pos[Grid.GetRow(border), Grid.GetColumn(border)] = 1;
                    }
                    else if (lastClicked.Fill == Brushes.Black)
                    {
                        checker_pos[Grid.GetRow(border), Grid.GetColumn(border)] = 2;
                    }
                }
            }
        }*/


    }
}
