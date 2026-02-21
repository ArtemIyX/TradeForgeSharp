using System.Net;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using TradeForge.Backend.Data.Middleware;
using TradeForge.Backend.Middleware;
using TradeForge.Backend.Services;
using TradeForge.Backend.Services.Ws;

namespace TradeForge.Backend.Controllers;

[ApiController]
[Route("ws")]
public class WsController(IWsHubService hub, WsMessageChannel channel, ILogger<WsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task Connect([FromQuery] string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        if (hub.IsConnected(userId))
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
            return;
        }

        var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();

        // Tell ExceptionHandlingMiddleware that the handshake is complete so it
        // knows to send a WS error frame instead of an HTTP ProblemDetails body
        // if anything throws beyond this point.
        HttpContext.Items[WsContextKeys.ActiveSocket] = ws;

        if (!hub.TryAddSocket(userId, ws))
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
            return;
        }

        var tcs = new TaskCompletionSource();
        await channel.Writer.WriteAsync((userId, ws, tcs));
        await tcs.Task;
    }
}