using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Сheckers
{
    class Game
    {
        private List<Step> steps;
        private List<IObserver> observers;

        private Table window;
        private CheckerTable table;
        private NetComponent net;
        const string SYNCHRONIZATION_NAME = "READY";
        private string HOME_HOST = "192.168.0.106";
        private string SYNCRONIZATION_HOST = "192.168.0.104";
        private Thread listener = null;

        private enum GameMode : int
        {
            Join = 0,
            Start = 1
        }

        public Game(Table window)
        {
            this.window = window;
            observers = new List<IObserver>();
            steps = new List<Step>();
            int userColor = 0;//TODO:  реализовать выбор цыета на форме
        }

        public Game(Table window, string local_host, string remote_host)
        {
            this.window = window;
            observers = new List<IObserver>();
            steps = new List<Step>();
            int userColor = 0;//TODO:  реализовать выбор цыета на форме
            HOME_HOST = local_host;
            SYNCRONIZATION_HOST = remote_host;
        }

        public void Start(int mode)
        {
            Dictionary<string, string> netConfig = new Dictionary<string, string>();
            netConfig.Add("message", SYNCHRONIZATION_NAME);
            netConfig.Add("port", "3000");
            netConfig.Add("host", HOME_HOST);

            net = new NetComponent();
            net.SetPort(3000);
            switch (mode)
            {
                case (int)GameMode.Start:
                    {
                        if (net.Recieve(netConfig) == false)
                        {
                            MessageBox.Show("Не удалось найти игрока!");

                        }
                        else
                        {
                            table = new CheckerTable(window, (int)GameMode.Start);
                            net.AddObserver(table);
                            table.AddObserver(net);
                            InitialTable();


                        }

                        break;
                    }
                case (int)GameMode.Join:
                    {
                        if (net.Synchronize(netConfig, SYNCRONIZATION_HOST) == false)
                        {
                            MessageBox.Show("Не удалось найти игрока!");
                        }
                        else
                        {
                            table = new CheckerTable(window, (int)GameMode.Join);
                            net.AddObserver(table);
                            table.AddObserver(net);
                            InitialTable();
                        }
                        break;
                    }
            }

            if (net.Syncronized)
            {
                listener = new Thread(() => { net.Listener(); });
                listener.Name = "Listener";
                listener.Start();
            }
        }

        ~Game()
        {
            if (listener != null)
                listener.Abort();
        }


        private void InitialTable()
        {
            table.SetupRowsAndColumns();
            table.SetupCells();
            table.SetupCheckers();
        }

    }
}
