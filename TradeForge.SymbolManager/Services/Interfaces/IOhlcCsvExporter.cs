using TradeForge.Core.Models;
using TradeForge.SymbolManager.Models;

namespace TradeForge.SymbolManager.Services.Interfaces;

public interface IOhlcCsvExporter
{
    public Task ExportAsync(
        CsvExportRequest request,
        CancellationToken cancellationToken = default);
}