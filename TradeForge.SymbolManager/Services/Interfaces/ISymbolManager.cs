using TradeForge.Core.Models;

namespace TradeForge.SymbolManager.Services.Interfaces;

public interface ISymbolManager
{
    public ICollection<InstrumentSettings> GetAllSymbols();
    public InstrumentSettings? GetSymbol(string symbol);
    public InstrumentSettings CreateSymbol(string symbol);
    public InstrumentSettings EditSymbol(string symbol, InstrumentSettings coverage);
    public void RenameSymbol(string oldSymbol, string newSymbol);
    public void DeleteSymbol(string symbol);
    public bool DoesSymbolExist(string symbol);
    public bool DoesSymbolHasData(string symbol);
    public int GetSymbolRowCount(string symbol);
    public InstrumentDataContainer? GetSymbolData(string symbol);
    public void ImportData(string symbol, List<OHLC> ohlc);
    public void ClearData(string symbol);
}