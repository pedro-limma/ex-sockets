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
            var resultSocket = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

            var property = Encoding.ASCII.GetString(buffer, 0, resultSocket.Count).ToLower();

            var retornoClient = new Dados().GetProperty(property);

            Console.WriteLine(retornoClient);

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

public class Dados
{
    public string Name { get; set; } = "Pedro";
    public string CPF { get; set; } = "00000000000";
    public string RG { get; set; } = "0000000";
    public string DataAniversario { get; set; } = new DateTime(2001, 9, 4).ToString("D");
    public string Matricula { get; set; } = "19070031";
    public string Curso { get; set; } = "BCC";


    public string GetProperty(string property)
    {
        var propriedade = property.ToLower();
        return property switch 
        {
            "name" => Name,
            "cpf" => CPF,
            "rg" => RG,
            "dataaniversario" => DataAniversario,
            "matricula" => Matricula,
            "curso" => Curso,
            _ => "Propriedade não encontrada"
        };
    }
}