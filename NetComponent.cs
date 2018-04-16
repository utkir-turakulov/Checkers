using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Сheckers
{
    class NetComponent : IObserver
    {
        public void Update()
        {
            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("192.168.28.2"), 3000);

                byte[] bytes = new byte[2048];


                Socket sender = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sender.Connect(ipEndPoint);

                string message = "Поле изменено!";

                byte[] msg = Encoding.UTF8.GetBytes(message);

                int byteMessage = sender.Send(msg);

                int byteReceived = sender.Receive(bytes);

                MessageBox.Show("\nОтвет от сервера: {0}\n\n", Encoding.UTF8.GetString(bytes, 0, byteReceived));
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }catch(SocketException ex)
            {
                MessageBox.Show("Не удается осуществить связь с соперником");
            }
            
        }
    }
}
