using Microsoft.Extensions.Logging;
using ProtobufOverUdpExample.Grpc;

namespace Listener;

public sealed class NotificationUdpMessageHandler(ILogger<StatusUdpMessageHandler> logger) : IUdpMessageHandler<Notification>
{
    public async Task HandleAsync(Notification message, CancellationToken token)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Received notification {Id} {Text}; Current Time: {CurrentTime}.", message.Id,
                message.Text, DateTimeOffset.UtcNow);
        }
        
        await Task.Delay(10000, token); // Heavy handler.
        
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Completed notification {Id} {Text}; Current Time: {CurrentTime}.", message.Id,
                message.Text, DateTimeOffset.UtcNow);
        }
    }
}
