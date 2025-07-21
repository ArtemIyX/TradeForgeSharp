using TradeForge.Core.Models;

using TradeForge.SymbolManager.Models;

namespace TradeForge.SymbolManager.Services.Interfaces;

public interface IOhlcCsvImporter
{
    public Task<IReadOnlyList<OHLC>> ImportAsync(
        CsvImportRequest request,
        CancellationToken cancellationToken = default);
}