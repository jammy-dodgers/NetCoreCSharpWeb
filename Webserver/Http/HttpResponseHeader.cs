using System;
using System.Collections.Generic;
using System.Text;

namespace Jambox.Web.Http
{
    public class HttpResponseHeader
    {
        public string StatusLine { get; set; }
        public int StatusCode { get; set; }
        private string _reasonPhrase;
        public string ReasonPhrase { get => _reasonPhrase;
            set => _reasonPhrase = "\"" + value.Replace("\r", "").Replace("\n", "").Replace("\"", "") + "\""; }
    }
}
