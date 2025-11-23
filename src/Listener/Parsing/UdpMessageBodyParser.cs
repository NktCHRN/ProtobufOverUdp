using System.Reflection;
using Common;
using Google.Protobuf;

namespace Listener.Parsing;

public static class UdpMessageBodyParser
{
    public static TMessage ParseBody<TMessage>(byte[] udpMessage) where TMessage : IMessage
    {
        var type = typeof(TMessage);
        var result = ParseBody(udpMessage, type);
        return (TMessage) result;
    }
    
    public static IMessage ParseBody(byte[] udpMessage, Type type)
    {
        var typeLength = HelperMethods.GetTypeLength(udpMessage);
        if (udpMessage.Length < Constants.TypeNameLengthBytesCount + typeLength)
        {
            throw new ArgumentException("Udp message is too short.", nameof(udpMessage));
        }

        if (!typeof(IMessage).IsAssignableFrom(type))
        {
            throw new ArgumentException($"The type {type.FullName} is not assignable from IMessage.", nameof(type));
        }

        var messageBody = udpMessage.AsSpan()[(Constants.TypeNameLengthBytesCount + typeLength)..];

        var parserProperty = type.GetProperty("Parser", BindingFlags.Static | BindingFlags.Public)
                             ?? throw new ArgumentException($"Could not find property Parser {type.FullName}.");
        var parser = parserProperty.GetValue(null) as MessageParser
                     ?? throw new InvalidOperationException("Parser is wrong type.");
        
        var result = parser.ParseFrom(messageBody);
        
        return result;
    }
}