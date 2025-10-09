using TradeForge.Core.Models;

namespace TradeForge.BacktestEngine.Models;

public class Account
{
    public string AccountId { get; set; } = string.Empty;
    public double Balance { get; set; } = 0.0;
    public double Equity { get; set; } = 0.0;
}