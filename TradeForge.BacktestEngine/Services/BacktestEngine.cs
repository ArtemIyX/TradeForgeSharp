using TradeForge.BacktestEngine.Models;
using TradeForge.Core.Models;

namespace TradeForge.BacktestEngine.Services;

public class BacktestEngine
{
    public Account? BacktestAccount { get; set; } = new Account();
    public BacktestStrategy? SelectedStrategy { get; protected set; } = null;
    
    public InstrumentSettings? SelectedInstrument { get; protected set; } = null;
    public List<OHLC> InsrumentData { get; protected set; } = new List<OHLC>();
}