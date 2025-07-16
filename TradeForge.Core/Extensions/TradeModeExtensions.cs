using TradeForge.Core.Enums;

namespace TradeForge.Core.Extensions;

public static class TradeModeExtensions
{
    public static string ToJsonString(this TradeMode tradeMode) =>
        tradeMode switch
        {
            TradeMode.Full      => "full",
            TradeMode.BuyOnly   => "buy_only",
            TradeMode.SellOnly  => "sell_only",
            TradeMode.CloseOnly => "close_only",
            TradeMode.Disabled  => "disabled",
            _ => throw new ArgumentOutOfRangeException(nameof(tradeMode), tradeMode, null)
        };

    public static TradeMode ToTradeMode(this string jsonTradeMode) =>
        jsonTradeMode?.ToLower() switch
        {
            "full"      => TradeMode.Full,
            "buy_only"  => TradeMode.BuyOnly,
            "sell_only" => TradeMode.SellOnly,
            "close_only"=> TradeMode.CloseOnly,
            "disabled"  => TradeMode.Disabled,
            _ => throw new ArgumentException($"Unknown trade mode '{jsonTradeMode}'.")
        };
}