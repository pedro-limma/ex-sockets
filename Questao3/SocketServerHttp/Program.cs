using System.Net;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();

var buffer = new byte[256];
app.Map("/", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    else
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        while (true)
        {

            var requestClientSocket = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

            var requestClient = Encoding.ASCII.GetString(buffer, 0, requestClientSocket.Count).ToLower();

            await webSocket.SendAsync(
                Encoding.ASCII.GetBytes(requestClient),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            await Task.Delay(1000);
        }
    }
});

await app.RunAsync();