namespace TradeForge.Backend.Data.Requests.Ws;

public record BaseRequest(
    string Action,
    object? Payload);