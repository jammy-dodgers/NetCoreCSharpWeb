# NetCoreCSharpWeb
Sometimes ASP.NET is a bit overkill for what you need.

Heavy WIP. Currently probably does not adhere to standards.

Inspired by sinatra (ruby) / jester (nim)

# Example
```csharp
var server = Server.New(ip: IPAddress.Any, port: 27015, caseInsensitive: true)
                .GET(@"/file/([^/]+)", rq =>
                { //bad example, probably vunerable to (Directory Traversal?) attack
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
server.Run();
```
