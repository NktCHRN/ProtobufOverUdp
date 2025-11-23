using Google.Protobuf;

namespace Listener.Parsing;

public interface IUdpMessageParser
{
    IMessage ParseMessage(byte[] udpMessage);
}
