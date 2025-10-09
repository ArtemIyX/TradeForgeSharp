using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using TradeForge.Core.Generic;

namespace TradeForge.Core.Enums;

[JsonConverter(typeof(EnumMemberJsonConverter<ROrderSide>))]
public enum ROrderSide
{
    [EnumMember(Value = "buy")] Buy,
    [EnumMember(Value = "sell")] Sell
}