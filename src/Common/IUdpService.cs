using System.Net.Sockets;
using Google.Protobuf;

namespace Common;

public interface IUdpService
{
    Task<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken);
    Task SendMessageAsync(IMessage message);
    Task SendMessageAsync(IMessage message, string ipAddress, int port);
}
