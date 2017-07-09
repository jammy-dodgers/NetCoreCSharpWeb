using System;
using Jambox.Web;
using System.Net;

namespace WebTestProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = Server.New(IPAddress.Any, 5000, caseInsensitive: true)
                .GET(@"/.+", rq =>
                {
                    rq.Response.Append("Hello World!\n");
                    rq.Response.Append($"");
                    rq.Send();
                }).Build();
            server.Run((ex) => Console.WriteLine(ex.Message));
        }
    }
}