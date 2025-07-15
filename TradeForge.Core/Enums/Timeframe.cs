using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeForge.Core.Enums
{
    public enum Timeframe
    {
        [Description("1 Minute")] M1,
        [Description("5 Minutes")] M5,
        [Description("15 Minutes")] M15,
        [Description("30 Minutes")] M30,
        [Description("1 Hour")] H1,
        [Description("4 Hours")] H4,
        [Description("Daily")] D,
        [Description("Weekly")] W,
        [Description("Monthly")] M
    }
}
