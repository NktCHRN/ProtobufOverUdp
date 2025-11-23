namespace Common;

public class UdpServiceOptions
{
    public ushort Port { get; set; }
    
    public string? DestinationIpAddress { get; set; }
    public ushort? DestinationPort { get; set; }
}
