using System.Net.Sockets;
using System.Threading.Channels;
using Listener.Containers;
using Listener.Dispatchers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Listener;

public sealed class UdpQueueProcessor(IOptions<UdpListenerOptions> optionsContainer, ILogger<UdpQueueProcessor> logger, IUdpMessageChannelContainer channelContainer, IUdpMessageDispatcher dispatcher) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channels = channelContainer.GetChannels();
        var workers = new List<Task>();
        foreach (var (type, channel) in channels)
        {
            workers.AddRange(
                Enumerable.Range(0, optionsContainer.Value.MaxDegreeOfParallelism)
                .Select(_ => RunWorkerAsync(type, channel, stoppingToken))
                .ToList());
        }
        logger.LogInformation("UDP queue processor started.");
        
        try
        {
            await Task.WhenAll(workers);
        }
        finally
        {
            logger.LogInformation("UDP queue processor completed.");
        }
    }
    
    private async Task RunWorkerAsync(Type type, Channel<UdpReceiveResult> channel, CancellationToken stoppingToken)
    {
        var reader = channel.Reader;

        try
        {
            await foreach (var received in reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await dispatcher.DispatchAsync(type, received, stoppingToken);
                }
                catch (OperationCanceledException operationCanceledException) when (
                    operationCanceledException.CancellationToken == stoppingToken ||
                    stoppingToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "UDP queue processor error occurred, continuing.");
                }
            }
        }
        catch (OperationCanceledException operationCanceledException) when (
            operationCanceledException.CancellationToken == stoppingToken ||
            stoppingToken.IsCancellationRequested)
        {
            
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Fatal error in UDP worker loop");
        }
    }
}
