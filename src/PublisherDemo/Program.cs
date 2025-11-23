using System.Text.Json.Serialization;
using Common;
using Microsoft.AspNetCore.Mvc;
using ProtobufOverUdpExample.Grpc;
using Publisher;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddUdpService(builder.Configuration);
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/api/status", async ([FromBody] SendToListenerRequest<StatusMessageRequest> request, [FromServices]IUdpService udpService, [FromServices]ILogger<Program> logger) =>
    {
        var message = new Status()
        {
            Description = request.Message.Description,
            Number = request.Message.Number,
            Code = (ProtobufOverUdpExample.Grpc.StatusCode) request.Message.Code
        };
        await udpService.SendMessageAsync(message, request.IpAddress, request.Port);
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Sent status message {Number} {Description} Code:{Code}; Current Time: {CurrentTime}.", message.Number,
                message.Description, message.Code, DateTimeOffset.UtcNow);
        }
        
        return Results.Ok();
    })
    .WithName("SendStatus");

app.MapPost("/api/notifications", async ([FromBody] SendToListenerRequest<NotificationRequest> request, [FromServices]IUdpService udpService, [FromServices]ILogger<Program> logger) =>
    {
        var message = new Notification()
        {
            Id = Guid.NewGuid().ToString(),
            Text = request.Message.Text,
        };
        await udpService.SendMessageAsync(message, request.IpAddress, request.Port);
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Sent notification {Id} {Text}; Current Time: {CurrentTime}.", message.Id,
                message.Text, DateTimeOffset.UtcNow);
        }
        
        return Results.Ok();
    })
    .WithName("SendNotification");

app.Run();
