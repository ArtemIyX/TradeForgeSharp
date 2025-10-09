using TradeForge.Core.Enums.Framework;

namespace TradeForge.Core.Models.Framework;

// Order Model (similar to MQL5 ORDER_* properties)
public class Order
{
    public string OrderId { get; set; } = Guid.NewGuid().ToString();
    public string AccountId { get; set; } = string.Empty;
    public DateTime TimeSetup { get; set; } = DateTime.MinValue;
    public DateTime? TimeFilled { get; set; } = null;
    public DateTime? TimeCanceled { get; set; } = null;
    
    public OrderType Type { get; set; }
    public OrderState State { get; set; } = OrderState.Pending;
    
    public string Symbol { get; set; } = string.Empty;
    public double Volume { get; set; } = 0.0;
    public double PriceOpen { get; set; } = 0.0;  // For market orders: execution price, For pending: trigger price
    public double? StopLoss { get; set; } = null;
    public double? TakeProfit { get; set; } = null;
    
    public string Comment { get; set; } = string.Empty;
    public int MagicNumber { get; set; } = 0;
    
    // Link to position if filled
    public string? PositionId { get; set; } = null;
}