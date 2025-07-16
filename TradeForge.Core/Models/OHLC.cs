namespace TradeForge.Core.Models;

public class OHLC
{
    public DateTime Timestamp { get; set; } = DateTime.MinValue;
    public decimal Open { get; set; } = 0;
    public decimal High { get; set; } = 0;
    public decimal Low { get; set; } = 0;
    public decimal Close { get; set; } = 0;
    public decimal Volume { get; set; } = 0;
}