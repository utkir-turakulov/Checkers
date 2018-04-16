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
    public partial class Table : Window 
    {

        public Table()
        {
            InitializeComponent();
            Game game = new Game(this);
            game.Start();
            //game.SetupRowsAndColumns();           
            //game.SetupCells();
            //game.SetupCheckers();
            //game.AddObserver(net);
        }

    }
}
