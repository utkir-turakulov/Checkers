using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Сheckers
{
    /// <summary>
    /// Логика взаимодействия для Table.xaml
    /// </summary>
    public partial class Table : UserControl 
    {

        private enum UserColor:int
        {
            White = 0,
            Black = 1
        }
        

        public Table()
        {

        }

        public Table(int mode)
        {

            InitializeComponent();
            //int selectedColor = 0;///TODO Реализовать возможность выбора цвета пользователя от которого будет зависеть начало хода 
            Game game = new Game(this);
            game.Start(mode);
            //game.SetupRowsAndColumns();           
            //game.SetupCells();
            //game.SetupCheckers();
            //game.AddObserver(net);
        }

    }
}
