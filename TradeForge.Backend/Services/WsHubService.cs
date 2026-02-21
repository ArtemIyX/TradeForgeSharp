using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace TradeForge.Backend.Services;

public interface IWsHubService
{
    public string AddSocket(WebSocket socket);
    public Task BroadcastAsync(string message);
    public Task RemoveSocketAsync(string id);

    public bool TryAddSocket(string userId, WebSocket socket);

    public bool IsConnected(string userId);
    public Task SendToAsync(string userId, string message);
    public Task CloseAllAsync();
}

public class WsHubService(ILogger<WsHubService> logger) : IWsHubService
{
    private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

    public bool IsConnected(string userId)
        => _sockets.TryGetValue(userId, out var ws) && ws.State == WebSocketState.Open;

    public bool TryAddSocket(string userId, WebSocket socket)
        => _sockets.TryAdd(userId, socket); // returns false if key already exists

    public string AddSocket(WebSocket socket)
    {
        var id = Guid.NewGuid().ToString();
        _sockets[id] = socket;
        return id;
    }

    public async Task CloseAllAsync()
    {
        var closeTasks = _sockets.Keys.Select(userId => RemoveSocketAsync(userId));
        await Task.WhenAll(closeTasks);
    }

    public async Task SendToAsync(string userId, string message)
    {
        if (_sockets.TryGetValue(userId, out var socket) && socket.State == WebSocketState.Open)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    public async Task BroadcastAsync(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        foreach (var (_, socket) in _sockets)
        {
            if (socket.State == WebSocketState.Open)
                await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    public async Task RemoveSocketAsync(string id)
    {
        if (_sockets.TryRemove(id, out var socket))
        {
            try
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Kicked",
                        CancellationToken.None // don't use stoppingToken here, it's already cancelled
                    );
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error closing socket for {UserId}", id);
            }
            finally
            {
                socket.Dispose();
            }
        }
    }
}