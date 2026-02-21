namespace TradeForge.Backend.Data.Responses;

public record BaseResponse(
    int StatusCode = 0,
    string Message = "",
    object? Payload = null,
    string ErrorMessage = ""
);