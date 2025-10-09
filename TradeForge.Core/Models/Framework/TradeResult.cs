namespace TradeForge.Core.Models.Framework;

public class TradeResult
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public string? OrderId { get; set; } = null;
    public string? PositionId { get; set; } = null;
    public string? DealId { get; set; } = null;
    public double ExecutionPrice { get; set; } = 0.0;

    public override string ToString()
    {
        var parts = new List<string>();
    
        parts.Add($"Success: {Success}");
    
        if (!string.IsNullOrEmpty(Message))
            parts.Add($"Message: {Message}");
        
        if (!string.IsNullOrEmpty(OrderId))
            parts.Add($"OrderId: {OrderId}");
        
        if (!string.IsNullOrEmpty(PositionId))
            parts.Add($"PositionId: {PositionId}");
        
        if (!string.IsNullOrEmpty(DealId))
            parts.Add($"DealId: {DealId}");
        
        if (ExecutionPrice != 0.0)
            parts.Add($"ExecutionPrice: {ExecutionPrice}");
        
        return $"TradeResult {{{string.Join(", ", parts)}}}";
    }
}