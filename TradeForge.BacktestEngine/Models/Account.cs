using TradeForge.Core.Models;

namespace TradeForge.BacktestEngine.Models;

public class Account
{
    public string AccountId { get; set; } = string.Empty;
    public double Balance { get; set; } = 0.0;
    public double Equity { get; set; } = 0.0;
    
    private readonly List<Order> _orders = new();
    private readonly List<Deal> _closedDeals = new();
    
    public IReadOnlyList<Order> Orders => _orders;
    public IReadOnlyList<Deal>  ClosedDeals => _closedDeals;
}