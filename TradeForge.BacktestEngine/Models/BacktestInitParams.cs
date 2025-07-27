using TradeForge.Core.Models;

namespace TradeForge.BacktestEngine.Models;

public class BacktestInitParams
{
    public double InitialBalance { get; init; }
    public double Spread { get; init; }
    public double Slippage { get; init; }

    public BacktestStrategy Strategy { get; init; } = null!;
    public InstrumentSettings Instrument { get; init; } = null!;
    public List<OHLC> Data { get; init; } = new List<OHLC>();
}