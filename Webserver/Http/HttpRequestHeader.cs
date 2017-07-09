using System;
using System.Collections.Generic;
using System.Text;

namespace Jambox.Web.Http
{
    class HttpRequestHeader
    {
        public HttpRequestMethod Method { get; private set; }
        public Uri RequestURI { get; private set; }
        public string HttpVersion { get; private set; }
    }
}
