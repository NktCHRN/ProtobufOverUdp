using System.Net.Sockets;
using Listener.Parsing;
using Microsoft.Extensions.DependencyInjection;

namespace Listener.Dispatchers;

public class UdpMessageDispatcher(IServiceProvider serviceProvider) : IUdpMessageDispatcher
{
    public async Task DispatchAsync(Type type, UdpReceiveResult udpReceiveResult, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var handlerType = typeof(IUdpMessageHandler<>).MakeGenericType(type);
        var handler = scope.ServiceProvider.GetService(handlerType)
                      ?? throw new InvalidOperationException($"No handler registered for type {handlerType}");

        var parsedMessage = UdpMessageBodyParser.ParseBody(udpReceiveResult.Buffer, type);

        var handleMethod = handlerType.GetMethod("HandleAsync", [type, typeof(CancellationToken)]);
        var result = handleMethod!.Invoke(handler, [parsedMessage, cancellationToken]);

        if (result is not Task task)
        {
            throw new InvalidOperationException("HandleAsync must return a Task.");
        }

        await task;
    }
}
