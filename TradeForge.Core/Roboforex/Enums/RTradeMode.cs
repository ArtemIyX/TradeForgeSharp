using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProtoBuf;
using TradeForge.Core.Generic;

namespace TradeForge.Core.Enums;

[ProtoContract]
[JsonConverter(typeof(EnumMemberJsonConverter<TradeMode>))]
public enum TradeMode
{
    [EnumMember(Value = "full")] Full,

    [EnumMember(Value = "buy_only")] BuyOnly,

    [EnumMember(Value = "sell_only")] SellOnly,

    [EnumMember(Value = "close_only")] CloseOnly,

    [EnumMember(Value = "disabled")] Disabled
}

