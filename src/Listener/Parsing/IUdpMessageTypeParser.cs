namespace Listener.Parsing;

public interface IUdpMessageTypeParser
{
    Type GetType(byte[] udpMessage);
}