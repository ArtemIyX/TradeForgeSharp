using System.Globalization;
using System.Text;
using System.Web;

namespace TradeForge.Core.Models.Requests;

public class ModifyDealRequest : AccountDealIdRequest
{
    public double? StopLoss { get; init; }
    public double? TakeProfit { get; init; }

    public override string FormUrlEncoded()
    {
        var sb = new StringBuilder(64);

        Append(sb, "stop_loss", StopLoss?.ToString(CultureInfo.InvariantCulture));
        Append(sb, "take_profit", TakeProfit?.ToString(CultureInfo.InvariantCulture));

        return sb.Length > 0 ? sb.ToString(1, sb.Length - 1) : string.Empty;
    }
}