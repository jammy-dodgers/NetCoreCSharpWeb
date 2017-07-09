using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections.Immutable;

using RequestDict = System.Collections.Immutable.ImmutableDictionary<System.Text.RegularExpressions.Regex, System.Action<Jambox.Web.Request>>;
namespace Jambox.Web
{
    public partial class Server
    {
        //Routes
        private RequestDict getRequestMap;
        private RequestDict postRequestMap;
        private RequestDict putRequestMap;
        private RequestDict deleteMapping;

        private Server()
        {
        }

        public static ServerBuilder New(bool caseInsensitive = false)
        {
            return new ServerBuilder(caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }
    }
}
