using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Linq;
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
            using (var creader = new StreamReader(client.GetStream()))
            using (var cwriter = new StreamWriter(client.GetStream()))
            {
                try
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
                        header.RequestURI = requestLineS[1];
                    }

                    while (!string.IsNullOrWhiteSpace(requestLine))
                    {
                        requestLine = creader.ReadLine();
                        if (string.IsNullOrEmpty(requestLine))
                            break;
                        var index = requestLine.IndexOf(':');
                        var hname = requestLine.Substring(0, index);
                        var data = requestLine.Remove(0, index + 1).TrimStart();
                        header.GetType().GetRuntimeProperty(hname.Replace("-", ""))?.SetValue(header, data);
                    }
                    Regex regex = null;
                    Action<Request> action = null;
                    switch (header.Method) {
                    case HttpRequestMethod.GET:
                    {
                        (regex, action) = getRequestMap.FirstOrDefault(x => x.Item1.IsMatch(header.RequestURI));
                        break;
                    }
                    case HttpRequestMethod.POST:
                    {
                        (regex, action) = postRequestMap.FirstOrDefault(x => x.Item1.IsMatch(header.RequestURI));
                        break;
                    }
                    }
                    if (action == null)
                        throw new HttpRequestException("Could not find suitable URL to route");
                    action(new Request() {
                        Header = header,
                        responseStream = cwriter,
                        Captures = regex.Match(header.RequestURI).Captures,
                        UserIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address
                    });
                    cwriter.Flush();
                    client.GetStream().Dispose();
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    cwriter.Write("An error has occured!");
                    cwriter.Flush();
                    client.Dispose();
                    errorHandler(ex);
                }
            }
        }
    }
}
