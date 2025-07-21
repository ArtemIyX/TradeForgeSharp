using CsvHelper.Configuration;

namespace TradeForge.SymbolManager.Models;

/// <summary>
/// Request that the caller passes to the service.
/// </summary>
public sealed class CsvImportRequest
{
    public required string FilePath { get; init; }
    public required ClassMap HeaderTemplate { get; init; }
    /*public required char Delimiter { get; init; }*/
    public IProgress<int>? Progress { get; init; }
}