using System;
using Jambox.Web;
using System.Net;
using System.Linq;
namespace WebTestProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = Server.New(ip: IPAddress.Any, port: 80, caseInsensitive: true)
                .GET(@"/(.+)?", rq =>
                {
                    Console.WriteLine($"GET from {rq.Header.UserAgent} at {rq.IP}");
                    rq.Response.Append($"Hello {rq.IP} ({rq.Header.UserAgent})\n");
                    rq.Response.Append($"You asked for {rq.Groups[1]}");
                    rq.ResponseHeader.ReasonPhrase = "hello world!";
                    rq.Send();
                })
                .POST(".+", rq =>
                {
                    Console.WriteLine(rq.Header);
                }).Build();
            server.Run(HandleException);
        }
        static void HandleException(Exception ex, string route)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }
    }
}