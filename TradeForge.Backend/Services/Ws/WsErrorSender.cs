using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TradeForge.Backend.Services.Ws;

public interface IWsErrorSender
{
    public Task SendAndCloseAsync(WebSocket ws, Exception ex, ILogger logger);
}

public class WsErrorSender(IHostEnvironment env) : IWsErrorSender
{
    public async Task SendAndCloseAsync(WebSocket ws, Exception ex, ILogger logger)
    {
        logger.LogError(ex, "Unhandled WebSocket exception");

        try
        {
            if (ws.State != WebSocketState.Open) return;

            var payload = new
            {
                statusCode = 500,
                message = "An unexpected server error occurred.",
                detail = env.IsProduction() ? null : ex.Message,
            };

            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            }));

            await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            await ws.CloseAsync(WebSocketCloseStatus.InternalServerError,
                "Internal server error", CancellationToken.None);
        }
        catch (Exception closeEx)
        {
            logger.LogWarning(closeEx, "Failed to send WS error frame cleanly");
        }
    }
}