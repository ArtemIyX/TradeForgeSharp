namespace TradeForge.BacktestEngine.Models;

public class BacktestError
{
    public Exception Exception { get; init; } = new Exception();
    public BacktestResult Snapshot { get; init; } = new BacktestResult();
}