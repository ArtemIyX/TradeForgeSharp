namespace TradeForge.Core.Models.Framework;

public class BacktestResult
{
    public List<Deal> Deals { get; set; } = new List<Deal>();
    public List<Order> Orders { get; set; } = new List<Order>();
    
    public double InitialBalance { get; set; } = 0.0;
    public double FinalBalance { get; set; } = 0.0;
    public double FinalEquity { get; set; } = 0.0;
    
    public double TotalProfit { get; set; } = 0.0;
    public double TotalLoss { get; set; } = 0.0;
    public double NetProfit { get; set; } = 0.0;
    
    public int TotalTrades { get; set; } = 0;
    public int WinningTrades { get; set; } = 0;
    public int LosingTrades { get; set; } = 0;
    
    public double WinRate { get; set; } = 0.0;
    public double ProfitFactor { get; set; } = 0.0;
    
    public double MaxDrawdown { get; set; } = 0.0;
    public double MaxDrawdownPercent { get; set; } = 0.0;
    
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}