using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace TradeForge.Backend.Services;

public interface IWsHubService
{
    public string AddSocket(WebSocket socket);
    public Task BroadcastAsync(string message, CancellationToken cancellationToken = default);
    public Task RemoveSocketAsync(string id, string reason, CancellationToken cancellationToken = default);

    public bool TryAddSocket(string userId, WebSocket socket);

    public bool IsConnected(string userId);
    public Task SendToAsync(string userId, string messag, CancellationToken cancellationToken = default);
    public Task CloseAllAsync(string reaso, CancellationToken cancellationToken = default);
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

    public async Task CloseAllAsync(string reason, CancellationToken cancellationToken = default)
    {
        var closeTasks = _sockets.Keys.Select(userId => RemoveSocketAsync(userId, reason, cancellationToken));
        await Task.WhenAll(closeTasks);
    }

    public async Task SendToAsync(string userId, string message, CancellationToken cancellationToken = default)
    {
        if (_sockets.TryGetValue(userId, out var socket) && socket.State == WebSocketState.Open)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);
        }
    }

    public async Task BroadcastAsync(string message, CancellationToken cancellationToken = default)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        foreach (var (_, socket) in _sockets)
        {
            if (socket.State == WebSocketState.Open)
                await socket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);
        }
    }

    public async Task RemoveSocketAsync(string id, string reason, CancellationToken cancellationToken = default)
    {
        if (_sockets.TryRemove(id, out var socket))
        {
            try
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, reason, cancellationToken);
                }
                else if (socket.State == WebSocketState.CloseReceived)
                {
                    // Client already sent close frame, we just need to send ours back
                    await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, reason, cancellationToken);
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