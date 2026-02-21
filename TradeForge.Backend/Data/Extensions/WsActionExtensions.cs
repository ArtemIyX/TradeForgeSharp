using System.Reflection;
using TradeForge.Backend.Data.Attributes;
using TradeForge.Backend.Services;
using TradeForge.Backend.Services.Handlers;

namespace TradeForge.Backend.Data.Extensions;

public static class WsActionExtensions
{
    public static IServiceCollection AddWsActions(this IServiceCollection services, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetExecutingAssembly();

        var handlerTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && t.GetCustomAttribute<WsActionAttribute>() is not null
                        && t.IsAssignableTo(typeof(IWsActionHandler)));

        var registry = new WsActionRegistry(services.BuildServiceProvider());

        foreach (var type in handlerTypes)
        {
            services.AddScoped(type); // register each handler as scoped
            registry.Register(type);
        }

        services.AddSingleton<IWsActionRegistry>(registry);
        return services;
    }
}