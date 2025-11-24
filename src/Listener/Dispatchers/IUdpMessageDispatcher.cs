using System.Net.Sockets;

namespace Listener.Dispatchers;

public interface IUdpMessageDispatcher
{
    Task DispatchAsync(Type type, UdpReceiveResult udpReceiveResult, CancellationToken cancellationToken);
}
