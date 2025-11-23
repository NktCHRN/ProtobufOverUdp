# ProtobufOverUdp
A small .NET helper library that makes it easy to **send and receive Protocol Buffers messages over UDP**.

It wraps `UdpClient` and `Google.Protobuf` behind a simple API, so you can focus on your message contracts instead of byte arrays, ports and parsing.

---

## 📦 Installation
Common package includes UdpService which is necessary to Send protobuf messages over UDP.\
NuGet link: [ProtobufOverUdp.Common](https://www.nuget.org/packages/ProtobufOverUdp.Common)
```bash
dotnet add package ProtobufOverUdp.Common
```

Listener package includes UdpListener and interface IUdpMessageHandler which is necessary to Receive protobuf messages over UDP and handle them properly.\
NuGet link: [ProtobufOverUdp.Listener](https://www.nuget.org/packages/ProtobufOverUdp.Listener)
```bash
dotnet add package ProtobufOverUdp.Listener
```

---

## 🔧 Usage
### 📤 Send protobuf over UDP
To send protobuf messages over UDP, download **ProtobufOverUdp.Common** NuGet package, then add necessary services in your Program.cs
```csharp 
builder.Services.AddUdpService(builder.Configuration);
```
Now, you can inject IUdpService and send protobuf messages via UDP this way:
```csharp 
await udpService.SendMessageAsync(message, request.IpAddress, request.Port);
```
If you have the same target IP address and port everywhere, you can specify them this way:
```csharp 
builder.Services.AddUdpService(builder.Configuration, opt =>
{
    opt.DestinationIpAddress = "127.0.0.1";
    opt.DestinationPort = 5123;
});
```
Now, you can simply use the method this way:
```csharp 
await udpService.SendMessageAsync(message);
```
### 📥 Receive protobuf over UDP
To receive protobuf messages over UDP, download **ProtobufOverUdp.Listener** NuGet package, then add necessary services in your Program.cs
```csharp 
builder.Services.AddUdpService(context.Configuration);
builder.Services.AddUdpListener();
```
Now, for each protobuf type you should have its own UdpMessageHandler. Implement them in a similar way (Notification here is your message):
```csharp 
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
```
Finally, register them in DI:
```csharp
builder.Services.AddUdpMessageHandler<Notification, NotificationUdpMessageHandler>();
```
Now, your application will listen to coming UDP messages and handle them properly. 

## ▶️ Demo
There are ListenerDemo and PublisherDemo projects. You can run them to see how this application works. 
```bash
dotnet run --project .\src\ListenerDemo\ListenerDemo.csproj --launch-profile "ListenerDemo"
```
```bash
dotnet run --project .\src\PublisherDemo\PublisherDemo.csproj --launch-profile "http"
```
Now, you can use Postman collection (download it from PostmanCollections folder or from [this link](https://www.postman.com/flight-operator-20409804/protobufoverudp/collection/szid0ho/publisherdemo-udp?action=share&creator=20224052))
