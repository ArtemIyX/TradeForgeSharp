using Microsoft.Extensions.Logging;
using TradeForge.Core.Models;
using TradeForge.Core.Services.Interfaces;
using TradeForge.SymbolManager.Services.Interfaces;

namespace TradeForge.SymbolManager.Services.Impl;

public class SymbolManagerService : ISymbolManager
{
    private readonly ILogger<SymbolManagerService> _logger;
    private readonly ITradeForgeSerializer<InstrumentSettings> _symbolSerializer;
    private readonly ITradeForgeSerializer<InstrumentDataContainer> _dataSerializer;

    //TODO: Folder mannager
    private static readonly string DataSymbolsFolder = Path.Combine(
        AppContext.BaseDirectory, // folder that contains the exe/dll
        "Data", "Symbols");

    public SymbolManagerService(ILogger<SymbolManagerService> logger,
        ITradeForgeSerializer<InstrumentSettings> symbolSerializer,
        ITradeForgeSerializer<InstrumentDataContainer> dataSerializer)
    {
        _logger = logger;
        _symbolSerializer = symbolSerializer;
        _dataSerializer = dataSerializer;
    }

    public ICollection<InstrumentSettings> GetAllSymbols()
    {
        if (!Directory.Exists(DataSymbolsFolder))
            return Array.Empty<InstrumentSettings>();

        // Enumerate → deserialize → collect into a List<T>
        return Directory
            .EnumerateFiles(DataSymbolsFolder, "*.sym")
            .Select(f => _symbolSerializer.Deserialize(File.ReadAllBytes(f)))
            .ToList(); // materialises the sequence
    }

    public InstrumentSettings? GetSymbol(string symbol)
    {
        if (!DoesSymbolExist(symbol))
            return null;

        string path = Path.Combine(DataSymbolsFolder, $"{symbol}.sym");
        InstrumentSettings settings = _symbolSerializer.Deserialize(File.ReadAllBytes(path));

        if (DoesSymbolHasData(symbol))
        {
            InstrumentDataContainer? ohlc = GetSymbolData(symbol);
            if (ohlc is not null)
            {
                settings.Summary = ohlc.Summary();
            }
            else
            {
                settings.Summary = new SymbolDataSummary();
            }
        }

        return settings;
    }

    public bool DoesSymbolExist(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            return false;

        // Turn the symbol into the file name you look for
        var fileName = $"{symbol}.sym";
        var fullPath = Path.Combine(DataSymbolsFolder, fileName);

        return File.Exists(fullPath); // true if .sym file is present
    }

    public InstrumentSettings CreateSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));

        Directory.CreateDirectory(DataSymbolsFolder); // ensures “Data/Symbols” exists

        var sc = new InstrumentSettings
        {
            Ticker = symbol
        };

        var fileName = Path.Combine(DataSymbolsFolder, $"{symbol}.sym");
        File.WriteAllBytes(fileName, _symbolSerializer.Serialize(sc));

        return sc;
    }

    public InstrumentSettings EditSymbol(string symbol, InstrumentSettings coverage)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));

        ArgumentNullException.ThrowIfNull(coverage);

        // ensure we do not silently change the file name
        if (!symbol.Equals(coverage.Ticker, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Symbol parameter and Ticker in coverage must match.", nameof(symbol));


        var fileName = Path.Combine(DataSymbolsFolder, $"{symbol}.sym");

        // throw if the symbol does not yet exist
        if (!DoesSymbolExist(symbol))
            throw new FileNotFoundException($"Symbol file '{symbol}.sym' not found.", fileName);

        File.WriteAllBytes(fileName, _symbolSerializer.Serialize(coverage));
        return coverage;
    }

    public void DeleteSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));

        var filePath = Path.Combine(DataSymbolsFolder, $"{symbol}.sym");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Symbol file '{symbol}.sym' not found.", filePath);

        File.Delete(filePath);
    }

    public bool DoesSymbolHasData(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            return false;

        var fileName = $"{symbol}.hd";
        var fullPath = Path.Combine(DataSymbolsFolder, fileName);

        return File.Exists(fullPath);
    }

    public int GetSymbolRowCount(string symbol)
    {
        throw new NotImplementedException();
    }

    public InstrumentDataContainer? GetSymbolData(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));

        var fileName = $"{symbol}.hd";
        var fullPath = Path.Combine(DataSymbolsFolder, fileName);

        if (!File.Exists(fullPath))
            return null;

        byte[] bytes = File.ReadAllBytes(fullPath);
        return _dataSerializer.Deserialize(bytes);
    }

    public void ImportData(string symbol, List<OHLC> ohlc)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));

        if (ohlc == null)
            throw new ArgumentNullException(nameof(ohlc));

        Directory.CreateDirectory(DataSymbolsFolder);

        var fileName = $"{symbol}.hd";
        var fullPath = Path.Combine(DataSymbolsFolder, fileName);

        var container = new InstrumentDataContainer
        {
            Ticker = symbol,
            OHLC = ohlc
        };

        File.WriteAllBytes(fullPath, _dataSerializer.Serialize(container));
    }

    public void ClearData(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty.", nameof(symbol));

        string fileName = $"{symbol}.hd";
        string fullPath = Path.Combine(DataSymbolsFolder, fileName);

        if (!DoesSymbolHasData(symbol))
            throw new FileNotFoundException($"Symbol data file '{symbol}.hd' not found.", fullPath);

        File.Delete(fullPath);
    }
}