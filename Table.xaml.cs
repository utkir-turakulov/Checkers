using System.Windows.Controls;

namespace Сheckers
{
    /// <summary>
    /// Логика взаимодействия для Table.xaml
    /// </summary>
    public partial class Table : UserControl
    {

        private enum UserColor : int
        {
            White = 0,
            Black = 1
        }

        public Table()
        {

        }

        /// <summary>
        /// Конструктор для тестового режима 
        /// </summary>
        public Table(int mode)
        {
            InitializeComponent();
            Game game = new Game(this);
            game.Start(mode);
        }

        public Table(int mode, string remote_ip, string local_ip)
        {
            InitializeComponent();
            Game game = new Game(this, local_ip, remote_ip);
            game.Start(mode);
        }
    }
}
