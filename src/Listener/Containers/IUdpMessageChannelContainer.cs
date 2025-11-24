using System.Net.Sockets;
using System.Threading.Channels;

namespace Listener.Containers;

public interface IUdpMessageChannelContainer
{
    IReadOnlyDictionary<Type, Channel<UdpReceiveResult>> GetChannels();
    Channel<UdpReceiveResult> GetChannel(Type type);
    ChannelWriter<UdpReceiveResult> GetWriter(Type type);
    void CompleteWriters();
}
