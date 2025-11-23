using Microsoft.Extensions.Logging;
using ProtobufOverUdpExample.Grpc;

namespace Listener;

public sealed class StatusUdpMessageHandler(ILogger<StatusUdpMessageHandler> logger) : IUdpMessageHandler<Status>
{
    public Task HandleAsync(Status message, CancellationToken token)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Received status message {Number} {Description} Code:{Code}; Current Time: {CurrentTime}.", message.Number,
                message.Description, message.Code, DateTimeOffset.UtcNow);
        }
        
        return Task.CompletedTask;
    }
}
