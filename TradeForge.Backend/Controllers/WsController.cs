using System.Net;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TradeForge.Backend.Data.Responses;
using TradeForge.Backend.Services;

namespace TradeForge.Backend.Controllers;

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

        try
        {
            var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();

            if (!hub.TryAddSocket(userId, ws))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                return;
            }

            // hand off to background service and return immediately
            await channel.Writer.WriteAsync((userId, ws));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error for user {UserId}", userId);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}