using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Connections;
using TradeForge.Backend.Data.Handlers;
using TradeForge.Backend.Data.Requests.Ws;
using TradeForge.Backend.Data.Responses;

namespace TradeForge.Backend.Services.Ws;

public class WsMessageService(
    WsMessageChannel channel,
    IWsHubService hub,
    IServiceScopeFactory scopeFactory,
    IWsErrorSender wsErrorSender,
    ILogger<WsMessageService> logger,
    IHostApplicationLifetime lifetime)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        lifetime.ApplicationStopping.Register(() =>
        {
            logger.LogInformation("Server shutting down, closing all WebSocket connections...");
            hub.CloseAllAsync("Server shutting down", stoppingToken).GetAwaiter().GetResult();
        });

        await foreach (var (userId, ws, tcs) in channel.Reader.ReadAllAsync(stoppingToken))
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await SendAsync(ws, new BaseResponse(StatusCode: 200, Message: "Connected"));
                    await HandleMessagesAsync(ws, userId, stoppingToken);
                }
                catch (Exception ex)
                {
                    await wsErrorSender.SendAndCloseAsync(ws, ex,
                        logger);
                }
                finally
                {
                    tcs.SetResult();
                }
            }, stoppingToken);
        }
    }

    private async Task HandleMessagesAsync(WebSocket ws, string userId, CancellationToken stoppingToken)
    {
        /*try
        {*/
        while (ws.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
        {
            /*try
            {*/
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

            await HandleActionAsync(ws, userId, request, stoppingToken);
            /*}
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
            }*/
        }
        /*}
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error handling message from {UserId}: {ExType}", userId, ex.GetType().Name);
        }
        finally
        {
            await hub.RemoveSocketAsync(userId, "HandleMessagesAsync", CancellationToken.None);
        }*/
    }

    private async Task HandleActionAsync(WebSocket ws, string userId, BaseRequest request, CancellationToken ct)
    {
        // Each message gets its own scope so handlers can depend on some services,
        // repositories, or any other scoped/transient service safely.
        await using var scope = scopeFactory.CreateAsyncScope();

        var registry = scope.ServiceProvider.GetRequiredService<IWsActionHandlerRegistry>();
        var handlerType = registry.Resolve(request.Action);

        if (handlerType is null)
        {
            logger.LogWarning("Unknown action {Action} from {UserId}", request.Action, userId);
            return;
        }

        var handler = (IWsActionHandler)scope.ServiceProvider.GetRequiredService(handlerType);
        await handler.HandleAsync(ws, userId, request, ct);
    }

    private async Task<string?> ReadMessageAsync(WebSocket ws, CancellationToken stoppingToken)
    {
        var buffer = new byte[1024 * 4];
        using var ms = new MemoryStream();

        WebSocketReceiveResult result;
        do
        {
            result = await ws.ReceiveAsync(buffer, stoppingToken);

            logger.LogDebug("Received frame: Type={Type}, Count={Count}, EndOfMessage={End}",
                result.MessageType, result.Count, result.EndOfMessage);

            if (result.MessageType == WebSocketMessageType.Close)
                return null;

            ms.Write(buffer, 0, result.Count);
        } while (!result.EndOfMessage);

        return Encoding.UTF8.GetString(ms.ToArray());
    }

    private async Task SendAsync(WebSocket ws, object response)
    {
        var json = JsonSerializer.Serialize(response);
        var bytes = Encoding.UTF8.GetBytes(json);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}