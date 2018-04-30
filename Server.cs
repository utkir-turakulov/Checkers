using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Сheckers
{
    class Server
    {
        private int PORT;
        private int MAX_CONNECTIONS;
        private long LISTEN_TIMEOUT = 120000;


        public Server(int port, int maxConnections)
        {
            PORT = port;
            MAX_CONNECTIONS = maxConnections;
        }


        public void Listen()
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, PORT);

            Socket socketListener = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                while(stopWatch.ElapsedMilliseconds != LISTEN_TIMEOUT)
                {
                    socketListener.Bind(iPEndPoint);
                    socketListener.Listen(MAX_CONNECTIONS);

                    MessageBox.Show("Ожидаем соединение через порт {0}", iPEndPoint.ToString());

                    Socket handler = socketListener.Accept();
                    string data = null;

                    byte[] bytes = new byte[2048];
                    int recievedBytes = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, recievedBytes);

                    MessageBox.Show("Полученный текст: " + data + "\n\n");

                    string reply = "Спасибо за запрос в " + data.Length.ToString() + " символов";

                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);

                    if (data.IndexOf("<TheEnd>") > -1)
                    {
                        Console.WriteLine("Сервер завершил соединение с клиентом.");
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                stopWatch.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                
            }
        }




    }
}
