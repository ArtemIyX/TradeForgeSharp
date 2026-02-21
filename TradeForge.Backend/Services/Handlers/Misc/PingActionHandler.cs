using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using TradeForge.Backend.Data.Attributes;
using TradeForge.Backend.Data.Requests.Ws;
using TradeForge.Backend.Data.Responses;
using TradeForge.Backend.Data.Responses.Ws;

namespace TradeForge.Backend.Services.Handlers.Misc;

[WsAction("ping")]
public class PingActionHandler : IWsActionHandler
{
    public async Task HandleAsync(WebSocket ws, string userId, BaseRequest request)
    {
        var response = new BaseResponse(StatusCode: (int)HttpStatusCode.OK, Message: "pong",
            Payload: new PingResponsePayload());
        var json = JsonSerializer.Serialize(response);
        var bytes = Encoding.UTF8.GetBytes(json);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}