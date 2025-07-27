using System.Text;
using System.Web;

namespace TradeForge.Core.Models.Requests;

public class AccountOrderIdRequest : AccountIdRequest
{
    public string OrderId { get; init; } = default!;

    public override string PathQuery(string basePath, string? existingQuery = null)
    {
        // start with the parent’s path (which already has account_id)
        var path = base.PathQuery(basePath, existingQuery);

        // append order_id
        var sb = new StringBuilder(path);
        sb.Append('&')
            .Append("order_id=")
            .Append(HttpUtility.UrlEncode(OrderId));

        return sb.ToString();
    }
}