using System;
using System.Collections.Generic;
using System.IO;
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
        }

        public StringBuilder Response { get; internal set; }
        public Http.HttpRequestHeader Header { get; internal set; }
        public CaptureCollection Captures { get; internal set; }
        internal StreamWriter responseStream;

        public void Send()
        {
            responseStream.Write(Response.ToString());
        }
    }
}
