using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeForge.Core.Enums;

namespace TradeForge.Core.Models
{
    public sealed class SymbolCoverage
    {
        public string Symbol { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Timeframe MinimalTimeframe { get; set; } = Timeframe.M1;

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public int TotalDays => (DateTo - DateFrom).Days + 1;
        public int TotalRecords { get; set; }

        public string Category { get; set; } = string.Empty;
    }
}
