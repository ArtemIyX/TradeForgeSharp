namespace TradeForge.Backend.Data.Middleware;

/// <summary>
/// Well-known keys written to <see cref="Microsoft.AspNetCore.Http.HttpContext.Items"/>
/// so that middleware can inspect WebSocket state without tight coupling to controllers.
/// </summary>
public static class WsContextKeys
{
    /// <summary>
    /// Set by <see cref="TradeForge.Backend.Controllers.WsController"/> immediately after
    /// <c>AcceptWebSocketAsync()</c> succeeds. The value is the live <see cref="System.Net.WebSockets.WebSocket"/>.
    /// The exception middleware reads this to decide whether to write a WS error frame
    /// instead of an HTTP ProblemDetails body.
    /// </summary>
    public const string ActiveSocket = "WS_ActiveSocket";
}