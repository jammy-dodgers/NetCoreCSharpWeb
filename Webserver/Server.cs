using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using Jambox.Web.Http;
using RequestList = System.Collections.Immutable.ImmutableList<(System.Text.RegularExpressions.Regex, System.Action<Jambox.Web.Request>)>;


namespace Jambox.Web
{
    public partial class Server
    {
        //Routes
        private RequestList getRequestMap;
        private RequestList postRequestMap;
        private RequestList putRequestMap;
        private RequestList deleteMapping;

        private TcpListener tcp;
        private Server()
        {
        }

        public static ServerBuilder New(IPAddress ip, int port, bool caseInsensitive = false)
        {
            return new ServerBuilder(ip, port, caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }
        public void Run(Action<Exception> errorHandler = null)
        {
            tcp.Start();
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
                    var header = new Http.HttpRequestHeader();
                    header.IsSimpleRequest = requestLineS.Length == 2;

                    if (requestLineS.Length < 2 || requestLineS.Length > 3)
                        throw new HttpRequestException("Malformed HTTP header");

                    /*Keep the parses in their own scope*/
                    {
                        header.Method = Enum.TryParse(requestLineS[0], out HttpRequestMethod httprqmethodparse) ? httprqmethodparse : throw new HttpRequestException($"Malformed HTTP header method {requestLineS[0]}");
                        header.RequestURI = Uri.TryCreate(requestLineS[1], UriKind.RelativeOrAbsolute, out Uri uriparse) ? uriparse : throw new HttpRequestException($"Malformed HTTP header URI {requestLineS[1]}");
                    }

                    while (!string.IsNullOrWhiteSpace(requestLine))
                    {
                        requestLine = creader.ReadLine();
                    }
                    (_, var action) = getRequestMap[0];
                    action(new Request() { Header = header, responseStream = cwriter });
                }
                client.Dispose();
            }
            catch (Exception ex)
            {
                using (var cwriter = new StreamWriter(client.GetStream()))
                    cwriter.Write("An error has occured!");
                client.Dispose();
                errorHandler(ex);
            }
        }
    }
}
