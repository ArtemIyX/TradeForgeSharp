

namespace TradeForge.Models;

public class ContextMenuItem
{
    public ContextMenuItem()
    {
        
    }

    public ContextMenuItem(string text, Func<Task>? func)
    {
        Text = text;
        OnClick = func;
    }
    public string Text { get; init; } = "";
    public Func<Task>? OnClick { get; init; }
}