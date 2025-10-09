using TradeForge.Core.Enums.Framework;

namespace TradeForge.Core.Models.Framework;

// Deal Model (similar to MQL5 DEAL_* properties) - Historical record
public class Deal
{
    public string DealId { get; set; } = Guid.NewGuid().ToString();
    public string PositionId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    
    public DateTime Time { get; set; } = DateTime.MinValue;
    public DealType Type { get; set; }
    public DealReason Reason { get; set; } = DealReason.Expert;
    
    public string Symbol { get; set; } = string.Empty;
    public double Volume { get; set; } = 0.0;
    public double Price { get; set; } = 0.0;
    
    public double Commission { get; set; } = 0.0;
    public double Swap { get; set; } = 0.0;
    public double Profit { get; set; } = 0.0;
    
    public string Comment { get; set; } = string.Empty;
    public int MagicNumber { get; set; } = 0;
}
