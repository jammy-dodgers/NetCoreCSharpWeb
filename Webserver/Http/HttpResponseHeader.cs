using System;
using System.Collections.Generic;
using System.Text;

namespace Jambox.Web.Http
{
    /// <summary>
    /// HTTP response header
    /// TODO: Add more standard fields
    /// </summary>
    public class HttpResponseHeader
    {
        /// <summary>
        /// HTTP version
        /// </summary>
        public string HttpVersion { get; set; }
        /// <summary>
        /// HTTP status code
        /// </summary>
        public int StatusCode { get; set; }
        private string _reasonPhrase;
        /// <summary>
        /// HTTP reason phrase
        /// </summary>
        public string ReasonPhrase { get => _reasonPhrase;
            set => _reasonPhrase = value.Replace("\r", "").Replace("\n", ""); }

        /// <summary>
        /// Location (for redirects)
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Last Modified (currently does nothing)
        /// </summary>
        public string LastModified { get; set; }
    }
}
