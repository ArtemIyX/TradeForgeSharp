using TradeForge.Core.Enums.Framework;

namespace TradeForge.Core.Models.Framework;

// Position Model (similar to MQL5 POSITION_* properties)
public class Position
{
    public string PositionId { get; set; } = Guid.NewGuid().ToString();
    public string AccountId { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    
    public DateTime TimeOpen { get; set; } = DateTime.MinValue;
    public DateTime? TimeClose { get; set; } = null;
    
    public DealType Type { get; set; }
    public double Volume { get; set; } = 0.0;
    public double PriceOpen { get; set; } = 0.0;
    public double? PriceClose { get; set; } = null;
    
    public double? StopLoss { get; set; } = null;
    public double? TakeProfit { get; set; } = null;
    
    public double Commission { get; set; } = 0.0;
    public double Swap { get; set; } = 0.0;
    public double Profit { get; set; } = 0.0;
    
    public string Comment { get; set; } = string.Empty;
    public int MagicNumber { get; set; } = 0;
    
    // Link to opening order
    public string OpeningOrderId { get; set; } = string.Empty;
}