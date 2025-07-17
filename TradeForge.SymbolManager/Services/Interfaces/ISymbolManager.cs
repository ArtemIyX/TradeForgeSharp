using TradeForge.Core.Models;

namespace TradeForge.SymbolManager.Services.Interfaces;

public interface ISymbolManager
{
    public ICollection<InstrumentSettings> GetAllSymbols();
    public InstrumentSettings? GetSymbol(string symbol);
    public InstrumentSettings CreateSymbol(string symbol);
    public InstrumentSettings EditSymbol(string symbol, InstrumentSettings coverage);
    public bool DeleteSymbol(string symbol);
    public bool DoesSymbolExist(string symbol);
}