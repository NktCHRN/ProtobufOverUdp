namespace Publisher;

public sealed record SendToListenerRequest<T>(T Message, ushort Port = 5089, string IpAddress = "127.0.0.1");
