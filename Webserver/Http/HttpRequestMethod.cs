using System;
using System.Collections.Generic;
using System.Text;

namespace Jambox.Web.Http
{
    /// <summary>
    /// HTTP Request Method
    /// </summary>
    public enum HttpRequestMethod
    {
        /// <summary>
        /// GET
        /// </summary>
        GET,
        /// <summary>
        /// HEAD
        /// </summary>
        HEAD,
        /// <summary>
        /// POST
        /// </summary>
        POST,
        /// <summary>
        /// PATCH
        /// </summary>
        PATCH,
        /// <summary>
        /// PUT
        /// </summary>
        PUT,
        /// <summary>
        /// DELETE
        /// </summary>
        DELETE,
        /// <summary>
        /// TRACE
        /// </summary>
        TRACE,
        /// <summary>
        /// CONNECT
        /// </summary>
        CONNECT,
        /// <summary>
        /// OPTIONS
        /// </summary>
        OPTIONS
    }
}
