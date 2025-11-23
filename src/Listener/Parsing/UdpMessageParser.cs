using Google.Protobuf;

namespace Listener.Parsing;

public class UdpMessageParser(IUdpMessageTypeParser typeParser) : IUdpMessageParser
{
    public IMessage ParseMessage(byte[] udpMessage)
    {
        var type = typeParser.GetType(udpMessage);
        var message = UdpMessageBodyParser.ParseBody(udpMessage, type);
        
        return message;
    }
}
