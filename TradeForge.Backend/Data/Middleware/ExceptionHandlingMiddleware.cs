using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TradeForge.Backend.Data.Middleware;

namespace TradeForge.Backend.Middleware;

/// <summary>
/// Centralized exception handler for both HTTP and WebSocket connections.
///
/// HTTP / pre-handshake WS:
///   Catches any unhandled exception and writes an RFC 7807 ProblemDetails JSON
///   response before the response is committed.
///
/// Post-handshake WS (response already committed):
///   Sends a structured error JSON frame down the open socket, then closes it
///   with WebSocketCloseStatus.InternalServerError so the client always gets
///   a meaningful signal rather than a silent drop.
/// </summary>
public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            // Check whether the WS handshake has already been accepted.
            // After AcceptWebSocketAsync the HTTP response is committed (status 101),
            // so we can no longer write ProblemDetails to it.
            var ws = context.WebSockets.IsWebSocketRequest
                     && context.Items.TryGetValue(WsContextKeys.ActiveSocket, out var raw)
                     && raw is WebSocket { State: WebSocketState.Open } openSocket
                ? openSocket
                : null;

            if (ws is not null)
            {
                await HandleWebSocketExceptionAsync(ws, ex, context.RequestAborted);
            }
            else if (!context.Response.HasStarted)
            {
                await HandleHttpExceptionAsync(context, ex);
            }
            else
            {
                // Response already started and no open WS — nothing we can write;
                // just ensure the exception is logged.
                logger.LogError(ex, "Unhandled exception after response started for {Path}", context.Request.Path);
            }
        }
    }

    // -------------------------------------------------------------------------
    // HTTP
    // -------------------------------------------------------------------------

    private async Task HandleHttpExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, title) = MapException(ex);

        logger.LogError(ex, "Unhandled HTTP exception: {Title} on {Method} {Path}",
            title, context.Request.Method, context.Request.Path);

        var problem = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Instance = context.Request.Path,
            // Only expose detail in non-production to avoid leaking internals
            Detail = env.IsProduction() ? null : ex.Message,
        };

        // Correlation id — populated by your correlation middleware if present
        if (context.TraceIdentifier is { Length: > 0 } traceId)
            problem.Extensions["traceId"] = traceId;

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        });

        await context.Response.WriteAsync(json);
    }

    // -------------------------------------------------------------------------
    // WebSocket (post-handshake)
    // -------------------------------------------------------------------------

    private async Task HandleWebSocketExceptionAsync(WebSocket ws, Exception ex, CancellationToken ct)
    {
        logger.LogError(ex, "Unhandled WebSocket exception");

        try
        {
            // Best-effort: send a structured error frame so the client knows what happened
            var errorPayload = new
            {
                statusCode = (int)HttpStatusCode.InternalServerError,
                message = "An unexpected server error occurred.",
                detail = env.IsProduction() ? null : ex.Message,
            };

            var json = JsonSerializer.Serialize(errorPayload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            });

            var bytes = Encoding.UTF8.GetBytes(json);

            if (ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(bytes, WebSocketMessageType.Text, endOfMessage: true, ct);

                await ws.CloseAsync(
                    WebSocketCloseStatus.InternalServerError,
                    "Internal server error",
                    CancellationToken.None // don't pass ct — it may already be cancelled
                );
            }
        }
        catch (Exception closeEx)
        {
            // Swallow — we're already in an error path; just log it
            logger.LogWarning(closeEx, "Failed to send error frame or close WebSocket cleanly");
        }
    }

    // -------------------------------------------------------------------------
    // Exception → HTTP status mapping
    // -------------------------------------------------------------------------

    private static (HttpStatusCode StatusCode, string Title) MapException(Exception ex) => ex switch
    {
        ArgumentNullException or ArgumentException    => (HttpStatusCode.BadRequest,          "Bad Request"),
        UnauthorizedAccessException                   => (HttpStatusCode.Unauthorized,         "Unauthorized"),
        KeyNotFoundException                          => (HttpStatusCode.NotFound,             "Not Found"),
        NotImplementedException                       => (HttpStatusCode.NotImplemented,       "Not Implemented"),
        OperationCanceledException                    => (HttpStatusCode.ServiceUnavailable,   "Request Cancelled"),
        TimeoutException                              => (HttpStatusCode.GatewayTimeout,       "Gateway Timeout"),
        _                                             => (HttpStatusCode.InternalServerError,  "Internal Server Error"),
    };
}