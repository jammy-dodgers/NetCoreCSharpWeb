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
using System.Text;

namespace Jambox.Web
{
    /// <summary>
    /// Web Server
    /// </summary>
    public class Server
    {
        //Routes
#pragma warning disable CS0649 //something about these not being assigned to
#pragma warning disable CS0169 //despite the fact that they are in serverbuilder
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
        /// <summary>
        /// Begin constructing a new server
        /// </summary>
        /// <param name="ip">IP address. Typically, IPAddress.Any</param>
        /// <param name="port">Port. Typically, 80</param>
        /// <param name="caseInsensitive">Should the routes be case sensitive? (Can be overriden manually on a per-route basis when defining routes)</param>
        /// <param name="majorErrorString">String to be sent to client when an internal server error occurs.</param>
        /// <returns>ServerBuilder</returns>
        public static ServerBuilder New(IPAddress ip, int port = 80, bool caseInsensitive = false, string majorErrorString = "Error 500 - Internal Server Error")
        {
            return new ServerBuilder(ip, port, caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None, majorErrorString);
        }
        /// <summary>
        /// Run the server with an optional provided errorhandler
        /// </summary>
        /// <param name="errorHandler"></param>
        public void Run(Action<Exception, string> errorHandler = null)
        {
            tcp.Start();
            while (true)
            {
                var task = tcp.AcceptTcpClientAsync();
                var client = task.GetAwaiter().GetResult();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                ServeClient(client, errorHandler);
#pragma warning restore CS4014
            }
        }
        /// <summary>
        /// Setup the query strings
        /// </summary>
        /// <param name="url">URL. If contains query strings, they will be removed.</param>
        /// <returns>Dictionary mapping for querystrings</returns>
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
        /// <summary>
        /// Asynchronously serves a client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="errorHandler"></param>
        /// <returns></returns>
        private async Task ServeClient(TcpClient client, Action<Exception, string> errorHandler)
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
                    string rawUri = requestLineS[1];
                    string procUri = rawUri;
                    /*Keep the parses in their own scope*/
                    {
                        header.Method = Enum.TryParse(requestLineS[0], out HttpRequestMethod httprqmethodparse) ? httprqmethodparse : throw new HttpRequestException($"Malformed HTTP header method {requestLineS[0]}");
                        querystring = SetupQueryStrings(ref procUri);
                        header.RequestURI = rawUri;
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
                        QueryStrings = querystring,
                        Url = procUri,
                        RawUrl = rawUri,
                        Response = new StringBuilder(),
                        ResponseHeader = new Http.HttpResponseHeader()
                        {
                            StatusCode = 200,
                            ReasonPhrase = "OK",
                            HttpVersion = "HTTP/1.1"
                        }
                    });
                    cwriter.Flush();
                    client.GetStream().Dispose();
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    cwriter.Write("HTTP/1.1 500 Internal Server Error\r\n\r\n" + MajorErrorString);
                    cwriter.Flush();
                    client.Dispose();
                    errorHandler?.Invoke(ex, route);
                }
            }
        }
    }
}
