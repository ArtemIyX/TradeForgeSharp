using System.Globalization;
using System.Text;
using TradeForge.Core.Enums;
using TradeForge.Core.Generic;

namespace TradeForge.Core.Models.Requests;

public class PlaceOrderRequest : UrlEncodedBody
{
    public string Ticker { get; init; } = default!;
    public OrderSide Side { get; init; }
    public int Volume { get; init; }
    public double? Price { get; init; }
    public long? Expiration { get; init; }
    public double? StopLoss { get; init; }
    public double? TakeProfit { get; init; }
    public OrderType Type { get; init; }
    
    public override string ToFormUrlEncoded()
    {
        var sb = new StringBuilder(256);
        Append(sb, "ticker", Ticker);
        Append(sb, "side", Side switch
        {
            OrderSide.Buy => "buy",
            OrderSide.Sell => "sell",
            _ => throw new InvalidOperationException($"Unsupported side: {Side}")
        });
        Append(sb, "volume", Volume.ToString(CultureInfo.InvariantCulture));
        Append(sb, "price", Price?.ToString(CultureInfo.InvariantCulture));
        Append(sb, "expiration", Expiration?.ToString(CultureInfo.InvariantCulture));
        Append(sb, "stop_loss", StopLoss?.ToString(CultureInfo.InvariantCulture));
        Append(sb, "take_profit", TakeProfit?.ToString(CultureInfo.InvariantCulture));
        Append(sb, "type", Type switch
        {
            OrderType.Market => "market",
            OrderType.Stop => "stop",
            OrderType.Limit => "limit",
            _ => throw new InvalidOperationException($"Unsupported type: {Type}")
        });

        // Remove the leading '&'
        return sb.Length > 0 ? sb.ToString(1, sb.Length - 1) : string.Empty;
    }
}