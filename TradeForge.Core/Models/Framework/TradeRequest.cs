using TradeForge.Core.Enums.Framework;

namespace TradeForge.Core.Models.Framework;

public class TradeRequest
{
    public OrderType Type { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public double Volume { get; set; }
    public double? Price { get; set; } = null;  // For pending orders
    public double? StopLoss { get; set; } = null;
    public double? TakeProfit { get; set; } = null;
    public string Comment { get; set; } = string.Empty;
    public int MagicNumber { get; set; } = 0;
}