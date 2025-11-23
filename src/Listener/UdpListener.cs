using System.Net.Sockets;
using Common;
using Listener.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Listener;

public sealed class UdpListener(ILogger<UdpListener> logger, IUdpService udpService, IServiceProvider serviceProvider, IUdpMessageTypeParser typeParser) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Udp status listener started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var received = await udpService.ReceiveAsync(stoppingToken);

                var type = typeParser.GetType(received.Buffer);
                
                await using var scope = serviceProvider.CreateAsyncScope();

                var handlerType = typeof(IUdpMessageHandler<>).MakeGenericType(type);
                var handler = scope.ServiceProvider.GetService(handlerType)
                    ?? throw new InvalidOperationException($"No handler registered for type {handlerType}");

                var parsedMessage = UdpMessageBodyParser.ParseBody(received.Buffer, type);
                
                var handleMethod = handlerType.GetMethod("HandleAsync", [type, typeof(CancellationToken)]);
                var result = handleMethod!.Invoke(handler, [parsedMessage, stoppingToken]);
                
                if (result is Task task)
                {
                    await task;
                }
                else
                {
                    throw new InvalidOperationException("HandleAsync must return a Task.");
                }
            }
            catch (OperationCanceledException)
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
                logger.LogError(ex, "UDP listener error, continuing.");

                if (ex is not ArgumentException)
                {
                    await Task.Delay(100, stoppingToken);
                }
            }
        }

        logger.LogInformation("Udp status listener completed");
    }
}
