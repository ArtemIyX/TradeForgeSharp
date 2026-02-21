// IWsActionHandlerRegistry.cs

using TradeForge.Backend.Data.Handlers;

namespace TradeForge.Backend.Services.Ws;

public interface IWsActionHandlerRegistry
{
    Type? Resolve(string action);
}


public class WsActionHandlerRegistry : IWsActionHandlerRegistry
{
    private readonly Dictionary<string, Type> _handlerMap;

    public WsActionHandlerRegistry(IEnumerable<WsHandlerRegistration> registrations)
    {
        _handlerMap = registrations.ToDictionary(r => r.Action, r => r.HandlerType);
    }

    public Type? Resolve(string action) =>
        _handlerMap.GetValueOrDefault(action);
}
// Internal registration record — built by the extension method below
public record WsHandlerRegistration(string Action, Type HandlerType);