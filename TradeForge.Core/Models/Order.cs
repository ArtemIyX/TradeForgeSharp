using System.Text.Json.Serialization;
using TradeForge.Core.Enums;
using TradeForge.Core.Generic;

namespace TradeForge.Core.Models;

public class Order
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

    [JsonPropertyName("type")]
    [JsonConverter(typeof(EnumMemberJsonConverter<OrderType>))]
    public OrderType Type { get; set; }

    [JsonPropertyName("filled_price")]
    public double? FilledPrice { get; set; }

    [JsonPropertyName("price")]
    public double? Price { get; set; }

    [JsonPropertyName("expiration")]
    public long? Expiration { get; set; }

    [JsonPropertyName("last_modified")]
    public long? LastModified { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    [JsonPropertyName("deals")]
    public List<string> Deals { get; set; } = new();

    [JsonPropertyName("status")]
    [JsonConverter(typeof(EnumMemberJsonConverter<OrderStatus>))]
    public OrderStatus Status { get; set; }
}