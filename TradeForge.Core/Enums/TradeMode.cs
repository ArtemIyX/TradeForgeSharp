using System.Text.Json.Serialization;
using ProtoBuf;

namespace TradeForge.Core.Enums;

[ProtoContract]
public enum TradeMode
{
    [JsonPropertyName("full")]
    Full,

    [JsonPropertyName("buy_only")]
    BuyOnly,

    [JsonPropertyName("sell_only")]
    SellOnly,

    [JsonPropertyName("close_only")]
    CloseOnly,

    [JsonPropertyName("disabled")]
    Disabled
}

