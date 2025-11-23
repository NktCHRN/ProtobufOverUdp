using Common;
using Listener;
using Microsoft.Extensions.Hosting;
using ProtobufOverUdpExample.Grpc;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    services.AddUdpService(context.Configuration);
    services.AddUdpListener();
    services.AddUdpMessageHandler<Status, StatusUdpMessageHandler>();
    services.AddUdpMessageHandler<Notification, NotificationUdpMessageHandler>();
});

var app = builder.Build();


app.Run();
