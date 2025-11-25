using System.Net.Sockets;
using Google.Protobuf;
using Microsoft.Extensions.Options;

namespace Common;

public class UdpService : IUdpService, IDisposable
{
    private UdpClient _udpClient;
    private readonly UdpServiceOptions _options;

    public UdpService(IOptions<UdpServiceOptions> optionsContainer)
    {
        _options = optionsContainer.Value;
        _udpClient = new UdpClient(optionsContainer.Value.Port);
    }

    public async Task<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken)
    {
        return await _udpClient.ReceiveAsync(cancellationToken);
    }

    public Task SendMessageAsync(IMessage message)
    {
        if (string.IsNullOrEmpty(_options.DestinationIpAddress) || !_options.DestinationPort.HasValue)
        {
            throw new InvalidOperationException("ReceiverIpAddress and ReceiverPort are required.");
        }
        
        return SendMessageAsync(message, _options.DestinationIpAddress, _options.DestinationPort.Value);
    }

    public async Task SendMessageAsync(IMessage message, string ipAddress, int port)
    {
        var data = message.ToUdpByteArray();
        await _udpClient.SendAsync(data, data.Length, ipAddress, port);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _udpClient.Dispose();
            _udpClient = null!;
        }
    }
}
