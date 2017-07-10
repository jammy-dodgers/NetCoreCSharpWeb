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
            var server = Server.New(ip: IPAddress.Any, port: 27015, caseInsensitive: true)
                .GET(@"/file/([^/]+)", rq =>
                {
                    rq.Response.Append(System.IO.File.ReadAllText($"files/{rq.Groups[1]}"));
                    if (rq.QueryStrings.Count > 0) foreach (var kvp in rq.QueryStrings)
                    {
                        rq.Response.AppendLine($"<br />{kvp.Key} = {kvp.Value ?? ""}");
                    }
                    rq.Send();
                })
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