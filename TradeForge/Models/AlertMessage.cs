namespace TradeForge.Models;

public class AlertMessage
{
    public string Text { get; init; } = string.Empty;
    public AlertType Type { get; init; } = AlertType.Info;
    public DateTime TimeStamp { get; init; } = DateTime.Now;
}

public enum AlertType
{
    Info,
    Warning,
    Error
}