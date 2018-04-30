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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            //UserControl control =   Table ;          

            switch (item.Header.ToString())
            {
                case "Start game":
                 
                    DockPanel.Children.Add(new Table((int)GameMode.Start));
                    break;
                case "Join":
                 //   DockPanel.Children.Clear();
                    DockPanel.Children.Add(new Table((int)GameMode.Join));
                    break;
                case "Show user list":
                   // DockPanel.Children.Clear();
                    DockPanel.Children.Add(new UserList());
                    break;
            }

        }

    }
}
