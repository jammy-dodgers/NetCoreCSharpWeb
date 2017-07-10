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
using System.Collections.Generic;

namespace Jambox.Web
{
    public class Server
    {
        //Routes
#pragma warning disable CS0649
#pragma warning disable CS0169
        internal RequestList getRouteMap;
        internal RequestList postRouteMap;
        internal RequestList putRouteMap;
        internal RequestList deleteRouteMap;
        internal string MajorErrorString;
        internal TcpListener tcp;
#pragma warning restore CS0649
#pragma warning restore CS0169
        internal Server()
        {
        }

        public static ServerBuilder New(IPAddress ip, int port = 80, bool caseInsensitive = false, string majorErrorString = "An error has occured")
        {
            return new ServerBuilder(ip, port, caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None, majorErrorString);
        }
        public void Run(Action<Exception, string> errorHandler = null)
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
        private Dictionary<string,string> SetupQueryStrings(ref string url)
        {
            Dictionary<string, string> querystring = null;
            /*Setup query strings*/
            if (url.Contains("?"))
            {
                querystring = new Dictionary<string, string>();
                var qry = url.Split('?');
                url = qry[0];
                if (qry[1].Contains("&"))
                {
                    var sepparams = qry[1].Split('&');
                    foreach (var keyvalpair in sepparams)
                    {
                        string key;
                        string value = null;
                        if (keyvalpair.Contains('='))
                        {
                            var kvp = keyvalpair.Split('=');
                            key = kvp[0];
                            value = kvp[1];
                        }
                        else
                        {
                            key = keyvalpair;
                        }
                        querystring.Add(key, value);
                    }
                }
                else
                {
                    string key;
                    string value = null;
                    if (qry[1].Contains('='))
                    {
                        var kvp = qry[1].Split('=');
                        key = kvp[0];
                        value = kvp[1];
                    }
                    else
                    {
                        key = qry[1];
                    }
                    querystring.Add(key, value);
                }
            }
            return querystring;
        }
        public async Task RunAsync(TcpClient client, Action<Exception, string> errorHandler)
        {
            string route = "";
            using (var creader = new StreamReader(client.GetStream()))
            using (var cwriter = new StreamWriter(client.GetStream()))
            {
                try
                {
                    var requestLine = await creader.ReadLineAsync();
                    var requestLineS = requestLine?.Split(' ') ?? new string[0];
                    var header = new Http.HttpRequestHeader();
                    header.IsSimpleRequest = requestLineS.Length == 2;

                    if (requestLineS.Length < 2 || requestLineS.Length > 3)
                    {
                        await cwriter.WriteLineAsync("HTTP/1.1 400 Bad Request\r\n\r\n");
                        throw new HttpRequestException("Malformed HTTP header");
                    }

                    var querystring = new Dictionary<string, string>();
                    /*Keep the parses in their own scope*/
                    {
                        header.Method = Enum.TryParse(requestLineS[0], out HttpRequestMethod httprqmethodparse) ? httprqmethodparse : throw new HttpRequestException($"Malformed HTTP header method {requestLineS[0]}");
                        querystring = SetupQueryStrings(ref requestLineS[1]);
                        header.RequestURI = requestLineS[1];
                    }
                    route = header.RequestURI;
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
                        (regex, action) = getRouteMap.FirstOrDefault(x => x.Item1.IsMatch(header.RequestURI));
                        break;
                    }
                    case HttpRequestMethod.POST:
                    {
                        (regex, action) = postRouteMap.FirstOrDefault(x => x.Item1.IsMatch(header.RequestURI));
                        break;
                    }
                    case HttpRequestMethod.PUT:
                    {
                        (regex, action) = putRouteMap.FirstOrDefault(x => x.Item1.IsMatch(header.RequestURI));
                        break;
                    }
                    case HttpRequestMethod.DELETE:
                    {
                        (regex, action) = deleteRouteMap.FirstOrDefault(x => x.Item1.IsMatch(header.RequestURI));
                        break;
                    }
                    }
                    if (action == null)
                    {
                        await cwriter.WriteLineAsync("HTTP/1.1 404 Not Found\r\n\r\n");
                        throw new HttpRequestException("Could not find suitable URL to route");
                    }
                    action(new Request() {
                        Header = header,
                        responseStream = cwriter,
                        Groups = regex.Match(header.RequestURI).Groups,
                        IP = ((IPEndPoint)client.Client.RemoteEndPoint).Address,
                        QueryStrings = querystring
                    });
                    cwriter.Flush();
                    client.GetStream().Dispose();
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    cwriter.Write(MajorErrorString);
                    cwriter.Flush();
                    client.Dispose();
                    errorHandler(ex, route);
                }
            }
        }
    }
}
