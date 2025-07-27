using System.Text;
using System.Web;

namespace TradeForge.Core.Generic;

public abstract class IUrlencodedBody
{
    public abstract string ToFormUrlEncoded();

    public virtual void Append(StringBuilder sb, string key, string? value)
    {
        if (value is null) return;
        sb.Append('&')
            .Append(HttpUtility.UrlEncode(key))
            .Append('=')
            .Append(HttpUtility.UrlEncode(value));
    }
}