using System.Globalization;
using System.Text;
using TradeForge.Core.Enums;
using TradeForge.Core.Generic;

namespace TradeForge.Core.Models.Requests;

public class RPlaceOrderRequest : RAccountIdRequest
{
    public string Ticker { get; init; } = default!;
    public ROrderSide Side { get; init; }
    public int Volume { get; init; }
    public double? Price { get; init; }
    public long? Expiration { get; init; }
    public double? StopLoss { get; init; }
    public double? TakeProfit { get; init; }
    public ROrderType Type { get; init; }
    
    public override string FormUrlEncoded()
    {
        var sb = new StringBuilder(256);
        Append(sb, "ticker", Ticker);
        Append(sb, "side", Side switch
        {
            ROrderSide.Buy => "buy",
            ROrderSide.Sell => "sell",
            _ => throw new InvalidOperationException($"Unsupported side: {Side}")
        });
        Append(sb, "volume", Volume.ToString(CultureInfo.InvariantCulture));
        Append(sb, "price", Price?.ToString(CultureInfo.InvariantCulture));
        Append(sb, "expiration", Expiration?.ToString(CultureInfo.InvariantCulture));
        Append(sb, "stop_loss", StopLoss?.ToString(CultureInfo.InvariantCulture));
        Append(sb, "take_profit", TakeProfit?.ToString(CultureInfo.InvariantCulture));
        Append(sb, "type", Type switch
        {
            ROrderType.Market => "market",
            ROrderType.Stop => "stop",
            ROrderType.Limit => "limit",
            _ => throw new InvalidOperationException($"Unsupported type: {Type}")
        });

        // Remove the leading '&'
        return sb.Length > 0 ? sb.ToString(1, sb.Length - 1) : string.Empty;
    }
}