using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Сheckers
{
    class NetComponent : IObserver, IObservable
    {
        private int PORT;
        private string IP;
        private string TRIGGER = "READY";

        private int MAX_CONNECTIONS = 10;
        private readonly long LISTEN_TIMEOUT = 120000;
        private int STATUS = 3;
        private bool connected = false;
        private int port;
        private List<IObserver> observers;

        private bool synchronized;

        public enum Statuses : int
        {
            Listen = 1,
            Sending = 2,
            Closed = 3,
            Synchronized = 4
        }

        public NetComponent()
        {
            observers = new List<IObserver>();
        }

        public bool Syncronized => synchronized;
        public int Status => STATUS;

        public bool Connected
        {
            get
            {
                return connected;
            }
        }

        public void SetPort(int port)
        {
            this.PORT = port;
        }

        public void Update(string message)
        {
            Socket sender = null;
            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(IP), 4000);

                byte[] bytes = new byte[2048];

                sender = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sender.Connect(ipEndPoint);


                byte[] msg = Encoding.UTF8.GetBytes(message);

                int byteMessage = sender.Send(msg);

                int byteReceived = sender.Receive(bytes);

                MessageBox.Show("\nОтвет от сервера: \n\n" + Encoding.UTF8.GetString(bytes, 0, byteReceived));
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Не удается осуществить связь с соперником! \n" + ex.Message);
            }
            finally
            {
                if (sender != null)
                {
                    // sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }

            }

        }

        public bool Synchronize(Dictionary<string, string> serverData, string host)
        {
            NetData netData = new NetData();
            netData.Message = serverData["message"];
            netData.Host = serverData["host"];
            netData.Port = serverData["port"];
            Dictionary<string, string> recieved = new Dictionary<string, string>();
            IP = host;
            Socket sender = null;
            string message = JsonConvert.SerializeObject(netData);

            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(host), PORT);

                byte[] bytes = new byte[2048];

                sender = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sender.Connect(ipEndPoint);

                STATUS = (int)Statuses.Synchronized;

                byte[] msg = Encoding.UTF8.GetBytes(message);

                int byteMessage = sender.Send(msg);

                int byteReceived = sender.Receive(bytes);
                string data = Encoding.UTF8.GetString(bytes, 0, byteReceived);

                MessageBox.Show("\nОтвет от сервера: " + data);

                if (data != null)
                {
                    netData = JsonConvert.DeserializeObject<NetData>(data);//десериализация данных
                    if (netData != null)
                    {
                        IP = netData.Host;
                        PORT = Convert.ToInt32(netData.Port);
                    }
                }
                connected = true;
                synchronized = true;
                return true;
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Не удается осуществить связь с соперником" + ex.Message);
                return false;
            }
            finally
            {
                if (sender != null)
                {
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
            }

        }

        public bool Recieve(Dictionary<string, string> serverData)
        {
            NetData netData = new NetData();
            netData.Message = serverData["message"];
            netData.Host = serverData["host"];
            netData.Port = serverData["port"];
            Dictionary<string, string> recieved = new Dictionary<string, string>();

            string message = JsonConvert.SerializeObject(netData);

            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, PORT);

            Socket socketListener = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket handler = null;
            Stopwatch stopWatch = new Stopwatch();
            try
            {
                string data = null;
                stopWatch.Start();
                while (stopWatch.ElapsedMilliseconds != LISTEN_TIMEOUT)
                {
                    socketListener.Bind(iPEndPoint);
                    socketListener.Listen(MAX_CONNECTIONS);
                    STATUS = (int)Statuses.Listen;
                    MessageBox.Show("Ожидаем соединение по адресу: ", iPEndPoint.ToString() + PORT);

                    handler = socketListener.Accept();


                    byte[] bytes = new byte[2048];
                    int recievedBytes = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, recievedBytes);

                    MessageBox.Show("Полученный текст: " + data + "\n\n");

                    if (data != null)
                    {
                        netData = JsonConvert.DeserializeObject<NetData>(data);//десериализация данных
                        if (netData != null)
                        {
                            IP = netData.Host;
                            PORT = Convert.ToInt32(netData.Port);
                        }
                    }

                    byte[] msg = Encoding.UTF8.GetBytes(message);
                    handler.Send(msg);
                    if (data != null)
                    {
                        connected = true;
                        synchronized = true;
                        return true;
                    }

                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                if (handler != null)
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    stopWatch.Stop();
                }
            }
        }


        public void Listener()
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 4000);

            Socket socketListener = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socketListener.Bind(iPEndPoint);
                socketListener.Listen(MAX_CONNECTIONS);

                while (true)
                {
                    MessageBox.Show("Ожидаем соединение через порт {0}" + iPEndPoint);

                    Socket handler = socketListener.Accept();
                    string data = null;

                    byte[] bytes = new byte[2048];
                    int recievedBytes = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, recievedBytes);

                    MessageBox.Show("Полученный текст: " + data + "\n\n");

                    Application.Current.Dispatcher.Invoke(new Action(() =>NotifyObservers(data)));
                   
                    string reply = "Recived";
                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);

                    if (data.IndexOf("<TheEnd>") > -1)
                    {
                        MessageBox.Show("Сервер завершил соединение с клиентом.");
                        break;
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                //Console.ReadLine();
            }
        }



        public List<IPAddress> GetLocalAddress()
        {

            List<IPAddress> ipList = new List<IPAddress>();
            // доступно ли сетевое подключение
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                return new List<IPAddress>();
            // запросить у DNS-сервера IP-адрес, связанный с именем узла
            var host = Dns.GetHostEntry(Dns.GetHostName());
            // Пройдем по списку IP-адресов, связанных с узлом
            foreach (var ip in host.AddressList)
            {
                // если текущий IP-адрес версии IPv4, то выведем его 
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipList.Add(ip);
                    Console.WriteLine(ip.ToString());
                }
            }
            return ipList;
        }

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
            foreach (IObserver o in observers)
            {
                o.Update(message);
            }
        }

        ~NetComponent()
        {
        }
    }
}
