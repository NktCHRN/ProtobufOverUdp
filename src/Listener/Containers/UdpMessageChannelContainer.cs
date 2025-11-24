using System.Net.Sockets;
using System.Threading.Channels;
using Microsoft.Extensions.Options;

namespace Listener.Containers;

public class UdpMessageChannelContainer : IUdpMessageChannelContainer
{
    private readonly Dictionary<Type, (Channel<UdpReceiveResult> Channel, ChannelWriter<UdpReceiveResult> Writer)> _channels = new();
    
    public UdpMessageChannelContainer(ISupportedTypesContainer supportedTypesContainer, IOptions<UdpListenerOptions> udpListenerOptions)
    {
        foreach (var type in supportedTypesContainer.SupportedTypes)
        {
            var channel = Channel.CreateBounded<UdpReceiveResult>(
                new BoundedChannelOptions(udpListenerOptions.Value.MaxItemsInQueue)
                {
                    SingleWriter = true,
                    SingleReader = false
                });
            _channels[type] = (channel, channel.Writer);
        }
    }

    public IReadOnlyDictionary<Type, Channel<UdpReceiveResult>> GetChannels() 
        => _channels.ToDictionary(x => x.Key, x => x.Value.Channel);

    public Channel<UdpReceiveResult> GetChannel(Type type)
    {
        return _channels[type].Channel;
    }
    
    public ChannelWriter<UdpReceiveResult> GetWriter(Type type)
    {
        return _channels[type].Writer;
    }

    public void CompleteWriters()
    {
        foreach (var (_, writer) in _channels.Values)
        {
            writer.Complete();
        }
    }
}
