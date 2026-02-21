// WsActionHandlerExtensions.cs

using System.Reflection;
using TradeForge.Backend.Data.Attributes;
using TradeForge.Backend.Data.Handlers;
using TradeForge.Backend.Services.Ws;

namespace TradeForge.Backend.Data.Extensions;

public static class WsActionHandlerExtensions
{
    /// <summary>
    /// Scans all loaded assemblies for classes decorated with [WsAction],
    /// registers each as a scoped IWsActionHandler, and wires up the registry.
    /// </summary>
    public static IServiceCollection AddWsActionHandlers(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var handlerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && t.GetCustomAttribute<WsActionAttribute>() is not null
                        && t.IsAssignableTo(typeof(IWsActionHandler)));

        foreach (var type in handlerTypes)
        {
            var action = type.GetCustomAttribute<WsActionAttribute>()!.Action;

            services.AddScoped(type);                                      // concrete type (resolved by registry)
            services.AddSingleton(new WsHandlerRegistration(action, type)); // action → type mapping
        }

        services.AddSingleton<IWsActionHandlerRegistry, WsActionHandlerRegistry>();
        return services;
    }
}