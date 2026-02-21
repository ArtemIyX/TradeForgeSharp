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
}

public class WsHubService : IWsHubService
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
            if (socket.State == WebSocketState.Open)
            {
                await socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Connection closed",
                    CancellationToken.None
                );
            }

            socket.Dispose();
        }
    }
}