using System.ComponentModel.DataAnnotations;

namespace Common;

public class UdpServiceOptions
{
    [Range(0, ushort.MaxValue)]
    public int Port { get; set; }
    
    public string? DestinationIpAddress { get; set; }
    [Range(0, ushort.MaxValue)]
    public int? DestinationPort { get; set; }
}
