using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

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
        internal StreamWriter responseStream;

        public void Send()
        {
            responseStream.Write(Response.ToString());
        }
    }
}
