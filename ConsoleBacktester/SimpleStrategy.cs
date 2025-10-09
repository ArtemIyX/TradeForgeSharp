using TradeForge.BacktestEngine.Enums;
using TradeForge.BacktestEngine.Models;
using TradeForge.BacktestEngine.Services;
using TradeForge.Core.Enums.Framework;
using TradeForge.Core.Models.Framework;

namespace ConsoleBacktester;

public class SimpleStrategy : BacktestStrategy
{
    private const int Magic = 2005;
    public override Task<bool> OnInit(BacktestEngine engine, Account account)
    {
        Console.WriteLine($"Strat initialized");
        return Task.FromResult(true);
    }

    public override async Task OnBar(BacktestEngine engine, Account account, int index, DateTime[] dates, double[] open,
        double[] high,
        double[] low, double[] close)
    {
        if (engine.GetPositions().Count == 0)
        {
            TradeResult result = engine.OrderSend(new TradeRequest()
            {
                Type = OrderType.Buy,
                Comment = $"Buy at {dates[index]}",
                MagicNumber = Magic,
                Symbol = "",
                Volume = 1.0
            });
            Console.WriteLine(result.ToString());
        }
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