using System.Text.Json.Serialization;
using TradeForge.Core.Enums;
using TradeForge.Core.Generic;

namespace TradeForge.Core.Models;

public class ROrder : ICloneable
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("ticker")]
    public string Ticker { get; set; } = default!;

    [JsonPropertyName("volume")]
    public int Volume { get; set; }

    [JsonPropertyName("side")]
    [JsonConverter(typeof(EnumMemberJsonConverter<ROrderSide>))]
    public ROrderSide Side { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(EnumMemberJsonConverter<ROrderType>))]
    public ROrderType Type { get; set; }

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

    public object Clone()
    {
        return new ROrder
        {
            Id           = this.Id,
            Ticker       = this.Ticker,
            Volume       = this.Volume,
            Side         = this.Side,
            Type         = this.Type,
            FilledPrice  = this.FilledPrice,
            Price        = this.Price,
            Expiration   = this.Expiration,
            LastModified = this.LastModified,
            Comment      = this.Comment,
            CreateTime   = this.CreateTime,
            Deals        = new List<string>(this.Deals),   // deep-copy list
            Status       = this.Status
        };
    }
}