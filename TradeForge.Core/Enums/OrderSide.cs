using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using TradeForge.Core.Generic;

namespace TradeForge.Core.Enums;

[JsonConverter(typeof(EnumMemberJsonConverter<OrderSide>))]
public enum OrderSide
{
    [EnumMember(Value = "buy")] Buy,
    [EnumMember(Value = "sell")] Sell
}