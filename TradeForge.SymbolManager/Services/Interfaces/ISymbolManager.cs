using TradeForge.Core.Models;

namespace TradeForge.SymbolManager.Services.Interfaces;

public interface ISymbolManager
{
    public ICollection<SymbolCoverage> GetAllSymbols();
    public SymbolCoverage? GetSymbol(string symbol);
    public SymbolCoverage CreateSymbol(string symbol);
    public SymbolCoverage EditSymbol(string symbol, SymbolCoverage coverage);
    public bool DeleteSymbol(string symbol);
    public bool DoesSymbolExist(string symbol);
}