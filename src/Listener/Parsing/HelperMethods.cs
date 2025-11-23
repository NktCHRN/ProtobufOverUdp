namespace Listener.Parsing;

public static class HelperMethods
{
    public static int GetTypeLength(byte[] udpMessage)
    {
        var typeLengthHigh = udpMessage[0];
        var typeLengthLow = udpMessage[1];
        var typeLength = typeLengthLow | (typeLengthHigh << 8);
        return typeLength;
    }
}
