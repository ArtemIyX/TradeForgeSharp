using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using TradeForge.Core.Generic;

namespace TradeForge.Core.Enums;

[JsonConverter(typeof(EnumMemberJsonConverter<DealStatus>))]
public enum DealStatus
{
    [EnumMember(Value = "open")] Open,
    [EnumMember(Value = "closing")] Closing,
    [EnumMember(Value = "closed")] Closed,
    [EnumMember(Value = "trade")] Trade
}