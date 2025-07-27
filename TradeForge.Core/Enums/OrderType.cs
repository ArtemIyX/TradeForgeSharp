using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using TradeForge.Core.Generic;

namespace TradeForge.Core.Enums;

[JsonConverter(typeof(EnumMemberJsonConverter<OrderType>))]
public enum OrderType
{
    [EnumMember(Value = "market")] Market,
    [EnumMember(Value = "stop")] Stop,
    [EnumMember(Value = "limit")] Limit,
    [EnumMember(Value = "stop_out")] StopOut,
    [EnumMember(Value = "stop_loss")] StopLoss,
    [EnumMember(Value = "take_profit")] TakeProfit
}