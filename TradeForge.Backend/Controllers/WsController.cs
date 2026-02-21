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

        // NOTE: IsConnected is a cheap early-exit for the common case (no duplicate).
        // It does NOT eliminate the race — two requests can both pass this check before
        // either calls AcceptWebSocketAsync. The real guard is TryAddSocket below.
        if (hub.IsConnected(userId))
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
            return;
        }

        // Accept the handshake. After this line the HTTP response is committed (101
        // Switching Protocols) — we can no longer set status codes or write HTTP bodies.
        // All error signalling from here on must go through the WebSocket itself.
        var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();

        // Register the open socket with the exception middleware so it knows to send
        // a WS error frame instead of an HTTP ProblemDetails body if anything throws.
        HttpContext.Items[WsContextKeys.ActiveSocket] = ws;

        // TryAddSocket is the real concurrency guard — ConcurrentDictionary.TryAdd is
        // atomic, so only one of two racing requests for the same userId will succeed.
        if (!hub.TryAddSocket(userId, ws))
        {
            logger.LogWarning("Duplicate connection attempt for {UserId} — closing the new socket", userId);

            // We can't write an HTTP response anymore, so close the socket explicitly
            // with a PolicyViolation code so the client gets a meaningful signal.
            await ws.CloseAsync(
                WebSocketCloseStatus.PolicyViolation,
                "A connection for this user already exists.",
                HttpContext.RequestAborted
            );
            return;
        }

        logger.LogInformation("WebSocket connected for {UserId}", userId);

        // Hand off to WsMessageService (BackgroundService) and park this request until
        // the connection closes. The TCS is completed by the background service when the
        // message loop exits, which unblocks the controller and releases the HTTP request.
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await channel.Writer.WriteAsync((userId, ws, tcs), HttpContext.RequestAborted);
        await tcs.Task;

        logger.LogInformation("WebSocket disconnected for {UserId}", userId);
    }
}