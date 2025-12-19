using CsvHelper.Configuration;
using TradeForge.Core.Models;

namespace TradeForge.SymbolManager.Models;

public class CsvExportRequest
{
    public required List<OHLC> OHLC { get; set; } = new List<OHLC>();
    public required string FilePath { get; init; }
    public required ClassMap HeaderTemplate { get; init; }
    public IProgress<int>? Progress { get; init; }
}