using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections.Immutable;

using RequestDict = System.Collections.Immutable.ImmutableSortedDictionary<System.Text.RegularExpressions.Regex, System.Action<Jambox.Web.Request>>;
using System.Net;
using System.Net.Sockets;

namespace Jambox.Web
{
    public partial class Server
    {
        //Routes
        private RequestDict getRequestMap;
        private RequestDict postRequestMap;
        private RequestDict putRequestMap;
        private RequestDict deleteMapping;

        private TcpListener tcp;
        private Server()
        {
            tcp = new TcpListener(addr, port);
        }

        public static ServerBuilder New(bool caseInsensitive = false)
        {
            return new ServerBuilder(caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        public void Run(IPAddress ip, int port)
        {
            Http.HttpListener listener = new Http.HttpListener(ip, port);
            var client = await tcp.AcceptTcpClientAsync();
            using (var creader = new StreamReader(client.GetStream()))
            using (var cwriter = new StreamWriter(client.GetStream()))
            {

            }
            client.Dispose();
        }
    }
}
