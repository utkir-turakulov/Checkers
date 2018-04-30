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
using Newtonsoft.Json;

namespace Сheckers
{
    class CheckerTable : IObservable, IObserver
    {
        private readonly int USERCOLOR;
        private readonly Border[,] _cells = new Border[8, 8];
        private readonly IShape[,] shapes = new IShape[8, 8];
        private readonly Ellipse[] _whiteCheckers = new Ellipse[12];
        private readonly Ellipse[] _blackCheckers = new Ellipse[12];

        private int[,] checker_pos = new int[8, 8];
        private int[] pos = new int[2];
        private int clickCounter = 0;

        private Ellipse lastClicked;
        private Border firstClicked;
        private Table window;


        private ShapeContainer[,] shapeContainer = new ShapeContainer[8, 8];

        private List<Step> steps;
        private List<IObserver> observers;

        private enum CheckerColor : int
        {
            Black = 0,
            White = 1

        }


        public CheckerTable(Table table, int userColor)
        {
            this.window = table;
            steps = new List<Step>();
            observers = new List<IObserver>();
            shapeContainer = new ShapeContainer[8, 8];
            USERCOLOR = userColor;

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
                    if (USERCOLOR != (int)CheckerColor.White)
                    {
                        _cells[row, pos + (row + 1) % 2].MouseLeftButtonDown -= MoveChecker;
                    }
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

                    if (USERCOLOR != (int)CheckerColor.Black)
                    {
                        _cells[row, pos + (row + 1) % 2].MouseLeftButtonDown -= MoveChecker;
                    }

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
            steps = new List<Step>();

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
                    if (checker != null && checker.GetType() == typeof(Checker))
                    {
                        newChecker = new Checker(_cells[Grid.GetRow(border), Grid.GetColumn(border)], shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)].GetColor());
                        shapes[newChecker.GetCell().Row, newChecker.GetCell().Coll] = newChecker;
                    }
                    if (checker != null && checker.GetType() == typeof(Queen))
                    {
                        newChecker = new Queen(_cells[Grid.GetRow(border), Grid.GetColumn(border)], shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)].GetColor());
                        shapes[newChecker.GetCell().Row, newChecker.GetCell().Coll] = newChecker;
                    }




                    if (newChecker != null && newChecker.GetCell().Row != newChecker.QueenSide()) //если шашка дошла до края противника
                    {
                        if (shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)].Move(border, _cells))//пытаемся выполнить действие
                        {
                            Cell to = new Cell();
                            to.Coll = Grid.GetColumn(border);
                            to.Row = Grid.GetRow(border);
                            Cell from = shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)].GetCell();

                            Step step = new Step(from, to, shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)]);
                            steps.Add(step);
                            shapes[checker.GetCell().Row, checker.GetCell().Coll] = null;
                            string message = JsonConvert.SerializeObject(steps);
                            NotifyObservers(message);// оповещаем наблюдателей 
                            steps.Clear();
                        }
                        else
                        {
                            shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)] = checker;
                            shapes[Grid.GetRow(border), Grid.GetColumn(border)] = null;
                        }
                    }
                    else
                    {

                        IShape oldQueen = shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)];
                        IShape newQueen = new Queen(_cells[Grid.GetRow(border), Grid.GetColumn(border)], shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)].GetColor());
                        shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)] = new Queen(_cells[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)], shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)].GetColor());

                        if (shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)].Move(border, _cells))
                        {
                            Cell to = new Cell();
                            to.Coll = Grid.GetColumn(border);
                            to.Row = Grid.GetRow(border);
                            Cell from = shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)].GetCell();
                            Step step = new Step(from, to, shapes[Grid.GetRow(firstClicked), Grid.GetColumn(firstClicked)]);
                            steps.Add(step);
                            string message = JsonConvert.SerializeObject(steps);

                            shapes[oldQueen.GetCell().Row, oldQueen.GetCell().Coll] = null;
                            shapes[Grid.GetRow(border), Grid.GetColumn(border)] = newQueen;

                            NotifyObservers(message);// оповещаем наблюдателей 
                            steps.Clear();
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

        public void NotifyObservers(string message)
        {
            //   string message = JsonConvert.SerializeObject(steps);

            foreach (IObserver observer in observers)
                observer.Update(message);
        }

        public void Update(string message)
        {
            // JsonConvert.
            List<Step> steps = JsonConvert.DeserializeObject<List<Step>>(message);
            foreach (Step step in steps)
            {
                _cells[step.From.Row, step.From.Coll] = _cells[step.To.Row, step.To.Coll];
                Border border = _cells[step.To.Row, step.To.Coll];

                shapes[step.From.Row, step.From.Coll].Move(border, _cells);
                shapes[step.From.Row, step.From.Coll] = null;
                _cells[step.From.Row, step.From.Coll] = null;
            }
        }
    }//CheckerTab
}//namespace
