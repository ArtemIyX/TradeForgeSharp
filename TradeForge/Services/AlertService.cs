using TradeForge.Models;

namespace TradeForge.Services;

// Services/IAlertService.cs
public interface IAlertService
{
    event Action<AlertMessage>? OnAlert;
    void ShowInfo(string text);
    void ShowWarning(string text);
    void ShowError(string text);
}

// Services/AlertService.cs
public class AlertService : IAlertService
{
    public event Action<AlertMessage>? OnAlert;

    public void ShowInfo(string text)  => Publish(text, AlertType.Info);
    public void ShowWarning(string text)=> Publish(text, AlertType.Warning);
    public void ShowError(string text) => Publish(text, AlertType.Error);

    private void Publish(string text, AlertType type)
        => OnAlert?.Invoke(new AlertMessage { Text = text, Type = type });
}