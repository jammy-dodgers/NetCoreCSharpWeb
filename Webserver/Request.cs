using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Jambox.Web
{
    /// <summary>
    /// Request Data
    /// </summary>
    public class Request
    {
        internal Request()
        {
        }

        /// <summary>
        /// HTTP Response text
        /// </summary>
        public StringBuilder Response { get; internal set; }
        /// <summary>
        /// HTTP Response header
        /// </summary>
        public Http.HttpResponseHeader ResponseHeader { get; set; }
        /// <summary>
        /// The requested URL (without provided querystrings)
        /// </summary>
        public string Url { get; internal set; }
        /// <summary>
        /// The requested URL (before querystring removal)
        /// </summary>
        public string RawUrl { get; internal set; }
        /// <summary>
        /// Query strings
        /// </summary>
        public Dictionary<string, string> QueryStrings { get; internal set; }
        /// <summary>
        /// HTTP Request header
        /// </summary>
        public Http.HttpRequestHeader Header { get; internal set; }
        /// <summary>
        /// The client's IP
        /// </summary>
        public IPAddress IP { get; internal set; }
        /// <summary>
        /// Regex captured groups
        /// </summary>
        public GroupCollection Groups { get; internal set; }
        internal StreamWriter responseStream;
        /// <summary>
        /// Send the response to the client
        /// </summary>
        public void Send()
        {
            responseStream.WriteLine($"{ResponseHeader.HttpVersion} {ResponseHeader.StatusCode} {ResponseHeader.ReasonPhrase}");
            responseStream.WriteLine();
            responseStream.Write(Response.ToString());
        }
        /// <summary>
        /// 302 Redirect the client to the specified URL.
        /// </summary>
        /// <param name="URL">URL to be redirected to</param>
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
