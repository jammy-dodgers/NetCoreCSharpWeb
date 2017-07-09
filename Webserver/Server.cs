using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections.Immutable;

using RequestDict = System.Collections.Immutable.ImmutableSortedDictionary<System.Text.RegularExpressions.Regex, System.Action<Jambox.Web.Request>>;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;

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
        }

        public static ServerBuilder New(IPAddress ip, int port, bool caseInsensitive = false)
        {
            return new ServerBuilder(caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }
        public void Run(Action<Exception> errorHandler = null)
        {
            while (true)
            {
                var task = tcp.AcceptTcpClientAsync();
                var client = task.GetAwaiter().GetResult();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                RunAsync(client, errorHandler);
#pragma warning restore CS4014
            }
        }
        public async Task RunAsync(TcpClient client, Action<Exception> errorHandler)
        {
            try
            {
                using (var creader = new StreamReader(client.GetStream()))
                using (var cwriter = new StreamWriter(client.GetStream()))
                {
                    var requestLine = await creader.ReadLineAsync();
                    var requestLineS = requestLine.Split(' ');
                    if (requestLineS.Length < 3)
                    {
                        throw new HttpRequestException("Request is missing Method, URI or HTTP version.");
                    }
                }
                client.Dispose();
            }
            catch (Exception ex)
            {
                errorHandler(ex);
            }
        }
    }
}
