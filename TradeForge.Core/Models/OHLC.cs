namespace TradeForge.Core.Models;

public class OHLC
{
    public DateTime Timestamp { get; set; } = DateTime.MinValue;
    public double Open { get; set; } = 0;
    public double High { get; set; } = 0;
    public double Low { get; set; } = 0;
    public double Close { get; set; } = 0;
    public double Volume { get; set; } = 0;
}