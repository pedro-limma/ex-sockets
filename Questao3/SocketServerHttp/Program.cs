using System.Net;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();

var data = new Dictionary<string, string>()
{
    { "nome", "Pedro"},
    { "cpf","12345678910" },
    { "rg",  "1234567"},
    { "datanascimento", new DateTime(2001, 9, 4).ToString("dd/MM/yyyy") },
    { "matricula", "19070031" },
    { "curso", "BCC"}
};


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

            var resultSocket = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

            var property = Encoding.ASCII.GetString(buffer, 0, resultSocket.Count).ToLower();

            var retornoClient = string.Empty;
            if (!data.ContainsKey(property))
                retornoClient = "Dado não encontrado";

            retornoClient = data[property];

            await webSocket.SendAsync(
                Encoding.ASCII.GetBytes(retornoClient),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            await Task.Delay(1000);
        }
    }
});

await app.RunAsync();