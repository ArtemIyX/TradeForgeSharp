namespace TradeForge.Core.Enums.Framework;

public enum DealReason
{
    Expert,      // Placed by EA/Strategy
    Manual,      // Manual trade
    StopLoss,    // Closed by SL
    TakeProfit   // Closed by TP
}