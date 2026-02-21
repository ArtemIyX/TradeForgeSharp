using System.Net.WebSockets;
using System.Threading.Channels;

namespace TradeForge.Backend.Services;

public class WsMessageChannel
{
    private readonly Channel<(string UserId, WebSocket Socket, TaskCompletionSource Tcs)> _channel =
        Channel.CreateUnbounded<(string, WebSocket, TaskCompletionSource)>();

    public ChannelWriter<(string UserId, WebSocket Socket, TaskCompletionSource Tcs)> Writer => _channel.Writer;
    public ChannelReader<(string UserId, WebSocket Socket, TaskCompletionSource Tcs)> Reader => _channel.Reader;
}