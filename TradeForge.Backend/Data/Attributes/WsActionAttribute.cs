namespace TradeForge.Backend.Data.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class WsActionAttribute(string action) : Attribute
{
    public string Action { get; } = action;
}
