using System.Text;
using System.Web;
using TradeForge.Core.Generic;

namespace TradeForge.Core.Models.Requests;

public class AccountIdRequest : UrlEncodedBody
{
    public string AccountId { get; init; } = default!;
    
    public override string FormUrlEncoded()
    {
        return string.Empty;
    }

    // put the account_id only into the query part
    public override string PathQuery(string basePath, string? existingQuery = null)
    {
        var sb = new StringBuilder(basePath);

        // append existing query, if any
        if (!string.IsNullOrEmpty(existingQuery))
            sb.Append(existingQuery);

        // append ? or & and account_id
        sb.Append(sb.ToString().Contains('?') ? '&' : '?')
            .Append("account_id=")
            .Append(HttpUtility.UrlEncode(AccountId));

        return sb.ToString();
    }
}