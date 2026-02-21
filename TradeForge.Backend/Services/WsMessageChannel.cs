using System.Net.WebSockets;
using System.Threading.Channels;

namespace TradeForge.Backend.Services;

public class WsMessageChannel
{
    private readonly Channel<(string UserId, WebSocket Socket)> _channel =
        Channel.CreateUnbounded<(string, WebSocket)>();

    public ChannelWriter<(string UserId, WebSocket Socket)> Writer => _channel.Writer;
    public ChannelReader<(string UserId, WebSocket Socket)> Reader => _channel.Reader;
}