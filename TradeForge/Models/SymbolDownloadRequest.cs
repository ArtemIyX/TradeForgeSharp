using TradeForge.Core.Enums;

namespace TradeForge.Models;

public record SymbolDownloadRequest(
    string Symbol,
    DateTime From,
    DateTime To,
    Timeframe Timeframe);