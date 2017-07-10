using System;
using System.Collections.Generic;
using System.Text;

namespace Jambox.Web.Http
{
    /// <summary>
    /// HTTP request header
    /// </summary>
    public class HttpRequestHeader
    {
        /// <summary>
        /// Is this a simple request? (Only method, URI and version)
        /// </summary>
        public bool IsSimpleRequest { get; internal set; }
        /* Main request line stuff */
        /// <summary>
        /// Method
        /// </summary>
        public HttpRequestMethod Method { get; internal set; }
        /// <summary>
        /// Requested URI
        /// </summary>
        public string RequestURI { get; internal set; }
        /// <summary>
        /// HTTP version
        /// </summary>
        public string HttpVersion { get; internal set; }

        /* Other  */
#pragma warning disable CS1591 //Missing XML comment
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
#pragma warning restore CS1591 //Missing XML comment

        /* Body */
        /// <summary>
        /// Request body
        /// </summary>
        public string Body { get; internal set; }
    }
}
