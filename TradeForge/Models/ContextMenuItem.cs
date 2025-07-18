namespace TradeForge.Models;

public class ContextMenuItem
{
    public string Text { get; init; } = "";
    public Func<Task>? OnClick { get; init; }
}