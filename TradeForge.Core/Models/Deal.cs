using System.Text.Json.Serialization;
using TradeForge.Core.Enums;
using TradeForge.Core.Generic;

namespace TradeForge.Core.Models;

public class Deal
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("ticker")]
    public string Ticker { get; set; } = default!;

    [JsonPropertyName("volume")]
    public int Volume { get; set; }

    [JsonPropertyName("side")]
    [JsonConverter(typeof(EnumMemberJsonConverter<OrderSide>))]
    public OrderSide Side { get; set; }

    [JsonPropertyName("open_price")]
    public double OpenPrice { get; set; }

    [JsonPropertyName("open_time")]
    public long OpenTime { get; set; }

    [JsonPropertyName("profit")]
    public double Profit { get; set; }

    [JsonPropertyName("close_price")]
    public double? ClosePrice { get; set; }

    [JsonPropertyName("close_time")]
    public long? CloseTime { get; set; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(EnumMemberJsonConverter<DealStatus>))]
    public DealStatus Status { get; set; }
}