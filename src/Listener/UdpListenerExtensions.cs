using Listener.Containers;
using Listener.Dispatchers;
using Listener.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Listener;

public static class UdpListenerExtensions
{
    public static IServiceCollection AddUdpListener(this IServiceCollection services)
        => AddUdpListener(services, _ => { });
    
    public static IServiceCollection AddUdpListener(this IServiceCollection services, Action<UdpListenerOptions> configure)
    {
        services.AddOptions<UdpListenerOptions>()
            .Configure(configure)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.TryAddSingleton<ISupportedTypesContainer, SupportedTypesContainer>();
        services.TryAddSingleton<IUdpMessageChannelContainer, UdpMessageChannelContainer>();
        services.TryAddSingleton<IUdpMessageTypeParser, UdpMessageTypeParser>();
        services.TryAddSingleton<IUdpMessageParser, UdpMessageParser>();
        services.AddHostedService<UdpListener>();
        
        services.TryAddSingleton<IUdpMessageDispatcher, UdpMessageDispatcher>();
        services.AddHostedService<UdpQueueProcessor>();
        
        return services;
    }

    public static IServiceCollection AddUdpMessageHandler<TMessage, TMessageHandler>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        where TMessageHandler : class, IUdpMessageHandler<TMessage>
    {
        services.Add(new ServiceDescriptor(typeof(IUdpMessageHandler<TMessage>), typeof(TMessageHandler), serviceLifetime));
        services.Add(new ServiceDescriptor(typeof(IUdpMessageHandler), typeof(TMessageHandler), serviceLifetime));
        return services;
    }
}
