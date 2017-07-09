using System;
using System.Collections.Generic;
using System.Text;

namespace Jambox.Web.Http
{
    public class HttpRequestHeader
    {
        /* Main request line stuff */
        public HttpRequestMethod Method { get; private set; }
        public Uri RequestURI { get; private set; }
        public string HttpVersion { get; private set; }

        /* Other  */
        public string Accept { get; internal set; }
        public string AcceptCharset { get; internal set; }
        public string AcceptEncoding { get; internal set; }
        public string AcceptLanguage { get; internal set; }
        public string Authorization { get; internal set; }
        public string Expect { get; internal set; }
        public string From { get; internal set; }
        public string Host { get; internal set; }
        public string IfMatch { get; internal set; }
        public string IfModifiedSince { get; internal set; }
        public string IfNoneMatch { get; internal set; }
        public string IfRange { get; internal set; }
        public string IfUnmodifiedSince { get; internal set; }
        public string MaxForwards { get; internal set; }
        public string ProxyAuthorization { get; internal set; }
        public string Range { get; internal set; }
        public string Referer { get; internal set; }
        public string TE { get; internal set; }
        public string UserAgent { get; internal set; }

        /* Body */
        public string Body { get; internal set; }
    }
}
