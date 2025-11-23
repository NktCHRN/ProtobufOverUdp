namespace Listener;

public interface IUdpMessageHandler<in T>
{
    Task HandleAsync(T message, CancellationToken token);
}
