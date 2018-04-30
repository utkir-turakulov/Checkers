using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows;

namespace Сheckers
{
    class UDPClient
    {
        private static IPAddress remoteAddress; // хост для отправки данных
        private readonly int remotePort; // порт для отправки данных
        private readonly int localPort; // локальный порт для прослушивания входящих подключений
        private long MESSAGE_SEND_TIMEOUT = 120000; //2 МИН
        private long MESSAGE_RECIVE_TIMEOUT = 180000; //3 МИН   

        public UDPClient(string host, int remotePort, int localPort)
        {
            remoteAddress = IPAddress.Parse(host);
            this.remotePort = remotePort;
        }


        public void SendMessage(string message)
        {
            UdpClient client = new UdpClient();
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Broadcast, remotePort);
            Stopwatch stopWatch = new Stopwatch();

            try
            {
                stopWatch.Start();
                while (stopWatch.ElapsedMilliseconds != MESSAGE_SEND_TIMEOUT)
                {
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    client.Send(data, data.Length, iPEndPoint); // отправка
                    Console.Write(" Sending ");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        public Dictionary<string, string> RecieveMessage()
        {
            UdpClient receiver = new UdpClient(localPort); // UdpClient для получения данных
            receiver.JoinMulticastGroup(remoteAddress, 20);
            IPEndPoint remoteIp = null;
            string localAddress = LocalIPAddress();
            NetData netData;
            Stopwatch stopwatch = new Stopwatch();

            Dictionary<string, string> dataDictionary = new Dictionary<string, string>();
            try
            {
                stopwatch.Start();
                while (stopwatch.ElapsedMilliseconds != MESSAGE_RECIVE_TIMEOUT)
                {
                    byte[] data = receiver.Receive(ref remoteIp); // получаем данные
                   // if (remoteIp.Address.ToString().Equals(localAddress))
                     //   continue;
                    string message = Encoding.Unicode.GetString(data);
                    Console.Write(" Listen ");
                    if (message.IndexOf("READY") > 0)
                    {
                        netData = JsonConvert.DeserializeObject<NetData>(message);
                        dataDictionary.Add("host", netData.Host);
                        dataDictionary.Add("port", netData.Port);
                        MessageBox.Show("Получено сообщение! "+message);
                        break;
                    }
                }
                return dataDictionary;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                receiver.Close();
            }
            return new Dictionary<string, string>();
        }

        private static string LocalIPAddress()
        {
            string localIP = "";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

    }
}
