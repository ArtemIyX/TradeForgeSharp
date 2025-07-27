using System.Text.Json.Serialization;
using TradeForge.BacktestEngine.Enums;

namespace TradeForge.BacktestEngine.Models;

[Serializable]
public class StrategyParameter
{
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public ParamType Type { get; init; }

    public object? Min { get; init; }
    public object? Max { get; init; }
    
    [JsonIgnore]
    public Type? EnumType { get; init; } // Only for enum
    
    public object? Value { get; set; } // Mutable
}