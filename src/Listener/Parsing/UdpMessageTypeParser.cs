using System.Collections.Frozen;
using System.Reflection;
using System.Text;
using Common;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace Listener.Parsing;

public class UdpMessageTypeParser : IUdpMessageTypeParser
{
    private readonly FrozenDictionary<string, Type> _protobufTypes;

    public UdpMessageTypeParser()
    {
        _protobufTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IMessage).IsAssignableFrom(t))
            .Select(t => new {
                Type = t,
                Descriptor = t
                    .GetProperty("Descriptor", BindingFlags.Public | BindingFlags.Static)
                    ?.GetValue(null) as MessageDescriptor
            })
            .Where(x => x.Descriptor != null)
            .ToDictionary(x => x.Descriptor!.FullName, x => x.Type)
            .ToFrozenDictionary();
    }

    public Type GetType(byte[] udpMessage)
    {
        if (udpMessage.Length <= Constants.TypeNameLengthBytesCount)
        {
            throw new ArgumentException("Udp message is too short.", nameof(udpMessage));
        }
        
        var typeLength = HelperMethods.GetTypeLength(udpMessage);

        if (udpMessage.Length < Constants.TypeNameLengthBytesCount + typeLength)
        {
            throw new ArgumentException("Udp message is too short.", nameof(udpMessage));
        }

        var typeNameBytes = udpMessage.AsSpan().Slice(Constants.TypeNameLengthBytesCount, typeLength);
        var typeName = Encoding.UTF8.GetString(typeNameBytes);

        var typeFound = _protobufTypes.TryGetValue(typeName, out var type);

        return typeFound 
            ? type!
            : throw new ArgumentException($"Could not find type {typeName}");
    }
}
