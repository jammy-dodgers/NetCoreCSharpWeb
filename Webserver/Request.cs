using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Jambox.Web
{
    public class Request
    {
        public Request()
        {
            Response = new StringBuilder();
            ResponseHeader = new Http.HttpResponseHeader()
            {
                StatusCode = 200,
                ReasonPhrase = "OK",
                HttpVersion = "HTTP/1.1"
            };
        }

        public StringBuilder Response { get; internal set; }
        public Http.HttpResponseHeader ResponseHeader { get; set; }
        public string Url { get; internal set; }
        public Dictionary<string, string> QueryStrings { get; internal set; }
        public Http.HttpRequestHeader Header { get; internal set; }
        public IPAddress IP { get; internal set; }
        public GroupCollection Groups { get; internal set; }
        internal StreamWriter responseStream;

        public void Send()
        {
            responseStream.WriteLine($"{ResponseHeader.HttpVersion} {ResponseHeader.StatusCode} {ResponseHeader.ReasonPhrase}");
            responseStream.WriteLine();
            responseStream.Write(Response.ToString());
        }
        public void Redirect(string URL)
        {
            ResponseHeader.Location = URL;
            ResponseHeader.StatusCode = 302;
            responseStream.WriteLine($"{ResponseHeader.HttpVersion} {ResponseHeader.StatusCode} {ResponseHeader.ReasonPhrase}");
            responseStream.WriteLine($"Location: {ResponseHeader.Location}");
            responseStream.WriteLine();
        }
    }
}
