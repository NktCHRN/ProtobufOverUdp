using Microsoft.Extensions.Logging;
using ProtobufOverUdpExample.Grpc;

namespace Listener;

public sealed class NotificationUdpMessageHandler(ILogger<StatusUdpMessageHandler> logger) : IUdpMessageHandler<Notification>
{
    public Task HandleAsync(Notification message, CancellationToken token)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Received notification {Id} {Text}; Current Time: {CurrentTime}.", message.Id,
                message.Text, DateTimeOffset.UtcNow);
        }
        
        return Task.CompletedTask;
    }
}
