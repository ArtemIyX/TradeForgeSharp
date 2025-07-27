using System.Globalization;
using System.Text;

namespace TradeForge.Core.Models.Requests;

public class ModifyOrderRequest : AccountOrderIdRequest
{
    public int? Volume { get; init; }
    public double? Price { get; init; }
    public long? Expiration { get; init; }
    public double? StopLoss { get; init; }
    public double? TakeProfit { get; init; }

    public override string FormUrlEncoded()
    {
        var sb = new StringBuilder(128);

        Append(sb, "volume", Volume?.ToString(CultureInfo.InvariantCulture));
        Append(sb, "price", Price?.ToString(CultureInfo.InvariantCulture));
        Append(sb, "expiration", Expiration?.ToString(CultureInfo.InvariantCulture));
        Append(sb, "stop_loss", StopLoss?.ToString(CultureInfo.InvariantCulture));
        Append(sb, "take_profit", TakeProfit?.ToString(CultureInfo.InvariantCulture));

        return sb.Length > 0 ? sb.ToString(1, sb.Length - 1) : string.Empty;
    }
}