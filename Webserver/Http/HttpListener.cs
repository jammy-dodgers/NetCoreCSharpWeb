using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Jambox.Web.Http
{
    sealed internal class HttpListener
    {
        public HttpListener(IPAddress addr, int port)
        {
            tcp = new TcpListener(addr, port);
        }
        TcpListener tcp;
    }
}
