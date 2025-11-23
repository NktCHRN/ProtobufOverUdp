namespace Publisher;

public sealed record StatusMessageRequest(int Number, string Description, StatusCode Code);
