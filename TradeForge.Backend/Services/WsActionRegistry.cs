using System.Reflection;
using TradeForge.Backend.Data.Attributes;
using TradeForge.Backend.Services.Handlers;

namespace TradeForge.Backend.Services;

public interface IWsActionRegistry
{
    IWsActionHandler? GetHandler(string action);
}

public class WsActionRegistry(IServiceProvider sp) : IWsActionRegistry
{
    private readonly Dictionary<string, Type> _map = [];

    public void Register(Type handlerType)
    {
        var attr = handlerType.GetCustomAttribute<WsActionAttribute>();
        if (attr is not null)
            _map[attr.Action] = handlerType;
    }

    public IWsActionHandler? GetHandler(string action)
        => _map.TryGetValue(action, out var type)
            ? (IWsActionHandler)sp.GetRequiredService(type)
            : null;
}