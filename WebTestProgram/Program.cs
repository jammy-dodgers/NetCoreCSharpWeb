using System;
using Jambox.Web;
using System.Net;

namespace WebTestProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = Server.New(ip: IPAddress.Any, port: 5000, caseInsensitive: true)
                .GET(@"/(.+)?", rq =>
                {
                    Console.WriteLine($"GET from {rq.Header.UserAgent} at {rq.UserIP}");
                    rq.Response.Append($"Hello {rq.UserIP}!\n");
                    rq.Send();
                }).Build();
            server.Run();
        }
        static void HandleException(Exception ex)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }
    }
}