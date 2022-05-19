using System.Net.WebSockets;
using System.Text;

using var ws = new ClientWebSocket();
await ws.ConnectAsync(new Uri("ws://localhost:5232/"), CancellationToken.None);

var client = new HttpClient();

var buffer = new byte[256];
while (ws.State == WebSocketState.Open)
{
    Console.WriteLine("Insira o tipo do request(caso não insira nada o padrão será post) [ GET,POST,PUT,DELETE ]: ");
    var inputMethod = Console.ReadLine();
    var method = SetRequestMethod(inputMethod.ToLower());

    Console.WriteLine("Insira a rota: ");
    var route = Console.ReadLine();

    var request = CreateRequest(method, route);

    var httpResponseMessage = await client.SendAsync(request);

    if (httpResponseMessage.IsSuccessStatusCode)
    {
        var content = await httpResponseMessage.Content.ReadAsStringAsync();

        await ws.SendAsync(
            Encoding.ASCII.GetBytes(content),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);

        var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Close)
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        else
            Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, result.Count)+"\n");
    }

}

HttpRequestMessage CreateRequest(HttpMethod method, string? route)
{
    return new HttpRequestMessage(method, route);
}

HttpMethod SetRequestMethod(string inputMethod)
{
    return inputMethod switch 
    { 
        "get" => HttpMethod.Get,
        "post" => HttpMethod.Post,
        "delete" => HttpMethod.Delete,
        "put" => HttpMethod.Put,
        _ => HttpMethod.Get,
    };
}