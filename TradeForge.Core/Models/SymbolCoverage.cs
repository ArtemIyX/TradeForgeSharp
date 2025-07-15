using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeForge.Core.Enums;

namespace TradeForge.Core.Models
{
    public sealed class ComparisonComparer<T> : IComparer<T>
    {
        private readonly Comparison<T> _cmp;
        public ComparisonComparer(Comparison<T> cmp) => _cmp = cmp;
        public int Compare(T? x, T? y) => _cmp(x!, y!);
    }

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

        public static Comparison<SymbolCoverage> GetComparer(string prop, bool desc)
        {
            Comparison<SymbolCoverage>? cmp = prop switch
            {
                nameof(SymbolCoverage.Symbol) => (a, b) => string.Compare(a.Symbol, b.Symbol, StringComparison.OrdinalIgnoreCase),
                nameof(SymbolCoverage.Description) => (a, b) => string.Compare(a.Description, b.Description, StringComparison.OrdinalIgnoreCase),
                nameof(SymbolCoverage.MinimalTimeframe) => (a, b) => a.MinimalTimeframe.CompareTo(b.MinimalTimeframe),
                nameof(SymbolCoverage.DateFrom) => (a, b) => a.DateFrom.CompareTo(b.DateFrom),
                nameof(SymbolCoverage.DateTo) => (a, b) => a.DateTo.CompareTo(b.DateTo),
                nameof(SymbolCoverage.TotalDays) => (a, b) => a.TotalDays.CompareTo(b.TotalDays),
                nameof(SymbolCoverage.TotalRecords) => (a, b) => a.TotalRecords.CompareTo(b.TotalRecords),
                nameof(SymbolCoverage.Category) => (a, b) => string.Compare(a.Category, b.Category, StringComparison.OrdinalIgnoreCase),
                _ => throw new ArgumentOutOfRangeException(nameof(prop))
            };

            if (desc)
            {
                var asc = cmp;       // capture
                cmp = (a, b) => asc(b, a); // reverse
            }

            return cmp;
        }
    }

}
