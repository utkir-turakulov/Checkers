using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Сheckers
{
    public class NetData
    {
        private string message;
        private string host;
        private string port;

        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }

        public string Host
        {
            get
            {
                return host;
            }
            set
            {
                host = value;
            }
        }

        public string Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
            }
        }
    }
}
