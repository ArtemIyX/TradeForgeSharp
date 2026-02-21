using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Connections;
using TradeForge.Backend.Data.Requests.Ws;
using TradeForge.Backend.Data.Responses;
using TradeForge.Backend.Data.Responses.Ws;

namespace TradeForge.Backend.Services;

public class WsMessageService(WsMessageChannel channel, IWsHubService hub, ILogger<WsMessageService> logger)
    : BackgroundService
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
        try
        {
            while (ws.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var message = await ReadMessageAsync(ws, stoppingToken);

                    if (message is null)
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", stoppingToken);
                        break;
                    }

                    logger.LogInformation("Message from {UserId}: {Message}", userId, message);

                    var request = JsonSerializer.Deserialize<BaseRequest>(message, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (request is null)
                    {
                        logger.LogWarning("Invalid message from {UserId}", userId);
                        continue;
                    }

                    await HandleActionAsync(ws, userId, request);
                }
                catch (WebSocketException ex)
                {
                    logger.LogWarning(ex, "WebSocket error for {UserId}", userId);
                    break;
                }
                catch (ConnectionAbortedException)
                {
                    logger.LogInformation("Client {UserId} disconnected", userId);
                    break;
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation("Connection cancelled for {UserId}", userId);
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error handling message from {UserId}", userId);
                    break;
                }
            }
        }
        finally
        {
            await hub.RemoveSocketAsync(userId);
        }
    }

    private async Task<string?> ReadMessageAsync(WebSocket ws, CancellationToken stoppingToken)
    {
        var buffer = new byte[1024 * 4];
        using var ms = new MemoryStream();

        WebSocketReceiveResult result;
        do
        {
            result = await ws.ReceiveAsync(buffer, stoppingToken);

            if (result.MessageType == WebSocketMessageType.Close)
                return null;

            ms.Write(buffer, 0, result.Count);
        }
        while (!result.EndOfMessage);

        return Encoding.UTF8.GetString(ms.ToArray());
    }

    private async Task HandleActionAsync(WebSocket ws, string userId, BaseRequest request)
    {
        switch (request.Action)
        {
            case "ping":
                var response = new BaseResponse(
                    StatusCode: (int)HttpStatusCode.OK,
                    Message: "pong",
                    Payload: new PingResponsePayload()
                );
                await SendAsync(ws, response);
                break;

            default:
                logger.LogWarning("Unknown action {Action} from {UserId}", request.Action, userId);
                break;
        }
    }

    private async Task SendAsync(WebSocket ws, object response)
    {
        var json = JsonSerializer.Serialize(response);
        var bytes = Encoding.UTF8.GetBytes(json);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}