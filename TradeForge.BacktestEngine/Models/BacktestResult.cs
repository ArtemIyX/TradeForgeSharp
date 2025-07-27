using TradeForge.Core.Models;

namespace TradeForge.BacktestEngine.Models;

public class BacktestResult
{
    public double FinalBalance { get; set; }
    public List<Deal> Deals { get; init; } = new List<Deal>();
}