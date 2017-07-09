using System;
using Jambox.Web;
using System.Net;

namespace WebTestProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = Server.New(caseInsensitive: true)
                .GET(@"/(\d+)\/([^\/]+)", rq =>
                {

                })
                .GET("/", rq =>
                {
                }).Build();
            server.RunAsync(IPAddress.Any, 80);
        }
    }
}