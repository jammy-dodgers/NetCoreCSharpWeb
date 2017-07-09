using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Jambox.Web
{
    public class Request
    {
        public Request()
        {
            Response = new StringBuilder();
        }

        public StringBuilder Response { get; private set; }

        private TcpClient respClient;

        public void Send()
        {

        }
    }
}
