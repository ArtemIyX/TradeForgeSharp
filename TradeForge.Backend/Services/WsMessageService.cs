using System.Net.WebSockets;
using System.Text;

namespace TradeForge.Backend.Services;

public class WsMessageService(WsMessageChannel channel, IWsHubService hub, ILogger<WsMessageService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var (userId, ws) in channel.Reader.ReadAllAsync(stoppingToken))
        {
            // fire and forget per connection, not blocking the loop
            _ = Task.Run(() => HandleMessagesAsync(ws, userId, stoppingToken), stoppingToken);
        }
    }

    private async Task HandleMessagesAsync(WebSocket ws, string userId, CancellationToken stoppingToken)
    {
        var buffer = new byte[1024 * 4];

        try
        {
            while (ws.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
            {
                var result = await ws.ReceiveAsync(buffer, stoppingToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", stoppingToken);
                    break;
                }

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    logger.LogInformation("Message from {UserId}: {Message}", userId, message);
                    //await hub.BroadcastAsync(message);
                }
            }
        }
        catch (WebSocketException ex)
        {
            logger.LogWarning(ex, "WebSocket error for user {UserId}", userId);
        }
        finally
        {
            await hub.RemoveSocketAsync(userId);
        }
    }
}