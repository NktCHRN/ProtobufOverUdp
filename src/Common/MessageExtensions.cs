using System.Text;
using Google.Protobuf;

namespace Common;

public static class MessageExtensions
{
    public static byte[] ToUdpByteArray(this IMessage message)
    {
        var typeName = message.Descriptor.FullName;
        if (typeName.Length > ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(message), "The type name is too long.");
        }
        
        var typeNameLengthLow = (byte) (typeName.Length & 0xFF);
        var typeNameLengthHigh = (byte) ((typeName.Length >> 8) & 0xFF);
        
        var typeNameBytes = Encoding.UTF8.GetBytes(typeName);

        var messageBytes = message.ToByteArray();
        
        var result = new byte[Constants.TypeNameLengthBytesCount + typeNameBytes.Length + messageBytes.Length];
        result[0] = typeNameLengthHigh;
        result[1] = typeNameLengthLow;
        typeNameBytes.CopyTo(result, Constants.TypeNameLengthBytesCount);
        messageBytes.CopyTo(result, Constants.TypeNameLengthBytesCount + typeNameBytes.Length);
        return result;
    }
}
