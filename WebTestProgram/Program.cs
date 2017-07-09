using System;
using Jambox.Web;
using System.Net;

namespace WebTestProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = Server.New(IPAddress.Any, 80, caseInsensitive: true)
                .GET(@"/(\d+)\/([^\/]+)", rq =>
                {

                })
                .GET("/", rq =>
                {
                }).Build();
            server.Run(ex => throw ex);
        }
    }
}