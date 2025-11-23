using Common;
using Google.Protobuf;
using Listener.Parsing;

namespace Listener;

public static class UdpServiceExtensions
{
    public static async Task<IMessage> ReceiveAsync(this IUdpService service, IUdpMessageParser parser, CancellationToken cancellationToken)
    {
        var response = await service.ReceiveAsync(cancellationToken);

        return parser.ParseMessage(response.Buffer);
    }
    
    public static async Task<TMessage> ReceiveAsync<TMessage>(this IUdpService service, CancellationToken cancellationToken) where TMessage : IMessage
    {
        var response = await service.ReceiveAsync(cancellationToken);

        var udpMessage = response.Buffer;
        
        return UdpMessageBodyParser.ParseBody<TMessage>(udpMessage);
    }
}
