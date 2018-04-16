using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Сheckers
{
    class CheckerTable : IObservable
    {
        private readonly Border[,] _cells = new Border[8, 8];
        private readonly IShape[,] shapes = new IShape[8, 8];
        private int[,] checker_pos = new int[8, 8];
        private readonly Ellipse[] _whiteCheckers = new Ellipse[12];
        private readonly Ellipse[] _blackCheckers = new Ellipse[12];
        private int clickCounter = 0;
        private int[] pos = new int[2];
        private Ellipse lastClicked;
        private Border firstClicked;
        private Table window;

        private ShapeContainer[,] shapeContainer = new ShapeContainer[8, 8];

        private List<Step> steps;
        private List<IObserver> observers;

        private enum CheckerColor : int
        {
            White = 0,
            Black = 1

        }


        public CheckerTable(Table table)
        {
            this.window = table;
            steps = new List<Step>();
            observers = new List<IObserver>();
            shapeContainer = new ShapeContainer[8, 8];
        }

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
                    shapes[row, pos + (row + 1) % 2] = new Checker(_cells[row, pos + (row + 1) % 2], Brushes.White.Color);
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
                    _cells[row, pos + (row + 1) % 2].Child = _blackCheckers[iBlack];
                    shapes[row, pos + (row + 1) % 2] = new Checker(_cells[row, pos + (row + 1) % 2], Brushes.Black.Color);
                    checker_pos[row, pos + (row + 1) % 2] = 2;
                    iBlack++;
                }
            }
        }

        public void MoveChecker(object sender, MouseButtonEventArgs e)
        {
            Border border = (Border)sender;
            Ellipse ellipse = (Ellipse)border.Child;

            if (ellipse != null)//если ячейка не пуста
            {
                if (clickCounter == 0)// если сделан первый клик, то
                {
                    firstClicked = border;// сохраняем ячейку фишки
                }

                clickCounter++;
            }
            else
            {
                clickCounter++;

                if (clickCounter == 2)
                {
                    IShape checker = shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)];
                    IShape newChecker = null;
                    if (checker != null && checker.GetType() ==  typeof(Checker))
                    {
                        newChecker = new Checker(_cells[Grid.GetRow(border), Grid.GetColumn(border)], shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)].GetColor());
                        shapes[newChecker.GetCell().Row, newChecker.GetCell().Coll] = newChecker;
                    }
                    if(checker != null && checker.GetType() == typeof(Queen))
                    {
                        newChecker = new Queen(_cells[Grid.GetRow(border), Grid.GetColumn(border)], shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)].GetColor());
                        shapes[newChecker.GetCell().Row, newChecker.GetCell().Coll] = newChecker;
                    }
                   

                   

                    if (newChecker != null && newChecker.GetCell().Row != newChecker.QueenSide()) //если шашка дошла до края противника
                    {
                        if (shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)].Move(border, _cells))//пытаемся выполнить действие
                        {

                            shapes[checker.GetCell().Row, checker.GetCell().Coll] = null;
                            NotifyObservers();// оповещаем наблюдателей 
                        }
                        else
                        {
                            shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)] = checker;
                            shapes[Grid.GetRow(border), Grid.GetColumn(border)] = null;
                        }
                    }
                    else
                    {

                        IShape oldQueen = shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)] ;
                        IShape newQueen = new Queen(_cells[Grid.GetRow(border), Grid.GetColumn(border)], shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)].GetColor()) ;
                        shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)] = new Queen(_cells[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)], shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)].GetColor());

                        if (shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)].Move(border, _cells))
                        {
                            shapes[oldQueen.GetCell().Row, oldQueen.GetCell().Coll] = null;
                            shapes[Grid.GetRow(border), Grid.GetColumn(border)] = newQueen;
                            NotifyObservers();// оповещаем наблюдателей 
                        }
                        else
                        {
                            shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)] = oldQueen;
                           // shapes[Grid.GetRow(border), Grid.GetColumn(border)] = null;
                        }

                    }
                }

                clickCounter = 0;
            }


        }//MoveChecker

        private SolidColorBrush GetColor(Color brush)
        {
            if (brush.Equals(Brushes.White))
            {
                return Brushes.Black;
            }
            if (brush.Equals(Brushes.Black))
            {
                return Brushes.White;
            }
            return new SolidColorBrush();
        }




        /**/
        public void AddObserver(IObserver o)
        {
            observers.Add(o);
        }

        public void RemoveObserver(IObserver o)
        {
            observers.Remove(o);
        }

        public void NotifyObservers()
        {
            foreach (IObserver observer in observers)
                observer.Update();
        }

    }//CheckerTab
}//namespace
