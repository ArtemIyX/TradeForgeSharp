using System.Net.WebSockets;
using TradeForge.Backend.Data.Requests.Ws;

namespace TradeForge.Backend.Data.Handlers;

public interface IWsActionHandler
{
    Task HandleAsync(WebSocket ws, string userId, BaseRequest request, CancellationToken ct = default);
}