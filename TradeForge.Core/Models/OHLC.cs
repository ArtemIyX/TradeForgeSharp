using ProtoBuf;

namespace TradeForge.Core.Models;

[ProtoContract]
public class OHLC
{
    [ProtoMember(1)] public DateTime Timestamp { get; set; } = DateTime.MinValue;
    [ProtoMember(2)] public double Open { get; set; } = 0;
    [ProtoMember(3)] public double High { get; set; } = 0;
    [ProtoMember(4)] public double Low { get; set; } = 0;
    [ProtoMember(5)] public double Close { get; set; } = 0;
    [ProtoMember(6)] public double Volume { get; set; } = 0;
}