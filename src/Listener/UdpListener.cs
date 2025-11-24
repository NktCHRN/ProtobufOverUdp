using System.Net.Sockets;
using Common;
using Listener.Containers;
using Listener.Parsing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Listener;

public sealed class UdpListener(ILogger<UdpListener> logger, IUdpService udpService, IUdpMessageChannelContainer channelContainer, IUdpMessageTypeParser typeParser) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("UDP listener started.");
       
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var received = await udpService.ReceiveAsync(stoppingToken);

                var type = typeParser.GetType(received.Buffer);

                var writer = channelContainer.GetWriter(type);
                
                await writer.WriteAsync(received, stoppingToken);
            }
            catch (OperationCanceledException operationCanceledException) when (
                operationCanceledException.CancellationToken == stoppingToken || stoppingToken.IsCancellationRequested)
            {
                // Expected on shutdown → exit loop
                break;
            }
            catch (SocketException se) when (
                se.SocketErrorCode is SocketError.Interrupted or SocketError.OperationAborted)
            {
                // Socket was closed because the host is stopping → exit loop
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UDP listener error occurred, continuing.");

                if (ex is not ArgumentException)
                {
                    await Task.Delay(100, stoppingToken);
                }
            }
        }

        channelContainer.CompleteWriters();
        logger.LogInformation("UDP listener completed.");
    }
}
