using Listener.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Listener;

public static class UdpListenerExtensions
{
    public static IServiceCollection AddUdpListener(this IServiceCollection services)
    {
        services.TryAddSingleton<IUdpMessageTypeParser, UdpMessageTypeParser>();
        services.TryAddSingleton<IUdpMessageParser, UdpMessageParser>();
        services.AddHostedService<UdpListener>();
        return services;
    }

    public static IServiceCollection AddUdpMessageHandler<TMessage, TMessageHandler>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        where TMessageHandler : class, IUdpMessageHandler<TMessage>
    {
        services.Add(new ServiceDescriptor(typeof(IUdpMessageHandler<TMessage>), typeof(TMessageHandler), serviceLifetime));
        return services;
    }
}
