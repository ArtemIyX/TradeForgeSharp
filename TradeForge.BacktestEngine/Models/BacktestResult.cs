using TradeForge.Core.Models;

namespace TradeForge.BacktestEngine.Models;

public class BacktestResult
{
    public List<Deal> Deals { get; init; } = new List<Deal>();
}