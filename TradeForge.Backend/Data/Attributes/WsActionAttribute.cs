namespace TradeForge.Backend.Data.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class WsActionAttribute(string action) : Attribute
{
    public string Action { get; } = action;
}