using System.ComponentModel.DataAnnotations;

namespace Listener;

public class UdpListenerOptions
{
    [Range(1, int.MaxValue)] 
    public int MaxDegreeOfParallelism { get; set; } = 1;
    [Range(1, int.MaxValue)] 
    public int MaxItemsInQueue { get; set; } = 1024;
}
