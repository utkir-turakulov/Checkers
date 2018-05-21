using System.Windows;
using System.Windows.Controls;

namespace Сheckers
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private enum GameMode : int
        {
            Join = 0,
            Start = 1
        }

        public MainWindow()
        {
            InitializeComponent();
            InitialEvents();
        }

        public void InitialEvents()
        {
            Start.Click += MenuItemClick;
            Join.Click += MenuItemClick;
            Exit.Click += MenuItemClick;
            User_list.Click += MenuItemClick;
        }

        public void MenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;

            if (!remote_ip.Text.Equals(""))
            {
                switch (item.Header.ToString())
                {
                    case "Start game":
                        TableRow.Children.Clear();
                        TableRow.Children.Add(element: new Table((int)GameMode.Start, remote_ip.Text, local_ip.Text)
                        {
                        });
                        break;
                    case "Join":
                        TableRow.Children.Clear();
                        TableRow.Children.Add(new Table((int)GameMode.Join, remote_ip.Text, local_ip.Text)
                        {
                        });
                        break;
                    case "Show user list":
                        TableRow.Children.Clear();
                        TableRow.Children.Add(new UserList());
                        break;
                }
            }
            else
            {
                MessageBox.Show("Не введен IP противника!!! \n Для синхронизации введите данные");
            }
        }
    }
}
