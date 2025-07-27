using TradeForge.BacktestEngine.Enums;
using TradeForge.BacktestEngine.Models;
using TradeForge.BacktestEngine.Services;

namespace ConsoleBacktester;

public class SimpleStrategy : BacktestStrategy
{
    public override Task<bool> OnInit(BacktestEngine engine, Account account)
    {
        Console.WriteLine($"Strat initialized");
        return Task.FromResult(true);
    }

    public override async Task OnBar(BacktestEngine engine, Account account, int index, DateTime[] dates, double[] open,
        double[] high,
        double[] low, double[] close)
    {
        await Task.Delay(1);
        Console.WriteLine($"Strat index {index} - {open[index]} {high[index]} {low[index]} {close[index]}");
    }

    protected override IEnumerable<StrategyParameter> DefineParameters()
    {
        yield return new StrategyParameter()
        {
            Name = "par1",
            DisplayName = "Int test param",
            Type = ParamType.Int,
            Min = 1,
            Max = 100,
            Value = 25
        };
    }

    public override string DisplayName => "Simple Strategy";
}