namespace Listener;

public interface IUdpMessageHandler<in T> : IUdpMessageHandler
{
    Task HandleAsync(T message, CancellationToken token);
}

public interface IUdpMessageHandler
{
    
}
