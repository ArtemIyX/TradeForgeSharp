namespace TradeForge.Backend.Data.Responses.Ws;

public record PingResponsePayload
{
    public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
}