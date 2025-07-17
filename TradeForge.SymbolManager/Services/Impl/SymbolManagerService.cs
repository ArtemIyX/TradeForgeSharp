using Microsoft.Extensions.Logging;
using TradeForge.Core.Models;
using TradeForge.Core.Services.Interfaces;
using TradeForge.SymbolManager.Services.Interfaces;

namespace TradeForge.SymbolManager.Services.Impl;

public class SymbolManagerService : ISymbolManager
{
    private readonly ILogger<SymbolManagerService> _logger;
    private readonly ITradeForgeSerializer<InstrumentSettings> _symbolSerializer;
    public List<InstrumentSettings> LoadedSymbols { get; set; } = new();

    //TODO: Folder mannager
    private const string Folder = "Data/Symbols";

    public SymbolManagerService(ILogger<SymbolManagerService> logger,
        ITradeForgeSerializer<InstrumentSettings> symbolSerializer)
    {
        _logger = logger;
        _symbolSerializer = symbolSerializer;
    }

    public ICollection<InstrumentSettings> GetAllSymbols()
    {
        if (!Directory.Exists(Folder))
            return Array.Empty<InstrumentSettings>();

        // Enumerate → deserialize → collect into a List<T>
        LoadedSymbols = Directory
            .EnumerateFiles(Folder, "*.sym")
            .Select(f => _symbolSerializer.Deserialize(File.ReadAllBytes(f)))
            .ToList(); // materialises the sequence
        return LoadedSymbols;
    }

    public InstrumentSettings? GetSymbol(string symbol)
    {
        throw new NotImplementedException();
    }

    public InstrumentSettings CreateSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));

        Directory.CreateDirectory(Folder); // ensures “Data/Symbols” exists

        var sc = new InstrumentSettings
        {
            Ticker = symbol
        };

        var fileName = Path.Combine(Folder, $"{symbol}.sym");
        File.WriteAllBytes(fileName, _symbolSerializer.Serialize(sc));

        return sc;
    }

    public InstrumentSettings EditSymbol(string symbol, InstrumentSettings coverage)
    {
        throw new NotImplementedException();
    }

    public bool DeleteSymbol(string symbol)
    {
        throw new NotImplementedException();
    }

    public bool DoesSymbolExist(string symbol)
    {
        throw new NotImplementedException();
    }
}