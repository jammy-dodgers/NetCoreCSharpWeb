using System;
using Jambox.Web;
namespace WebTestProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = Server.New(caseInsensitive: true)
                .GET("/.+$", rq =>
                {

                })
                .GET("", rq =>
                {
                }).Build();
        }
    }
}