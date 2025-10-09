using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using TradeForge.Core.Generic;

namespace TradeForge.Core.Enums;

[JsonConverter(typeof(EnumMemberJsonConverter<OrderStatus>))]
public enum OrderStatus
{
    [EnumMember(Value = "in_execution")] InExecution,
    [EnumMember(Value = "active")] Active,
    [EnumMember(Value = "rejected")] Rejected,
    [EnumMember(Value = "filled")] Filled,
    [EnumMember(Value = "canceled")] Canceled
}