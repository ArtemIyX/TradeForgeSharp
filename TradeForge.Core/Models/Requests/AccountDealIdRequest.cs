using System.Text;
using System.Web;

namespace TradeForge.Core.Models.Requests;

public class AccountDealIdRequest : AccountIdRequest
{
    public string DealId { get; init; } = default!;

    public override string PathQuery(string basePath, string? existingQuery = null)
    {
        var path = base.PathQuery(basePath, existingQuery);
        var sb = new StringBuilder(path);
        sb.Append('&')
            .Append("deal_id=")
            .Append(HttpUtility.UrlEncode(DealId));
        return sb.ToString();
    }
}