using ProtoBuf;
using TradeForge.Core.Enums;
using TradeForge.Core.Extensions;

namespace TradeForge.Core.Models;

using System.Text.Json.Serialization;

public sealed class ComparisonComparer<T> : IComparer<T>
{
    private readonly Comparison<T> _cmp;
    public ComparisonComparer(Comparison<T> cmp) => _cmp = cmp;
    public int Compare(T? x, T? y) => _cmp(x!, y!);
}

/*
"ticker": "EURUSD",
"description": "EUR/USD",
"contract_size": 1,
"units": "Share(s)"
"min_volume": 1,
"max_volume": 100000000,
"volume_step": 0.01,
"min_tick": 0.00001,
"leverage": 0.05,
"trade_mode": "full"
 */

public record SymbolDataSummary
{
    public SymbolDataSummary()
    {
    }

    public SymbolDataSummary(SymbolDataSummary otherSummary)
    {
        this.MinimalTimeframe = otherSummary.MinimalTimeframe;
        this.DateFrom = otherSummary.DateFrom;
        this.DateTo = otherSummary.DateTo;
    }

    public Timeframe MinimalTimeframe { get; init; } = Timeframe.M1;
    public DateTime DateFrom { get; init; } = DateTime.MinValue;
    public DateTime DateTo { get; init; } = DateTime.MinValue;
    public int TotalRecords { get; init; } = 0;

    public int TotalDays => DateTo == DateTime.MinValue
        ? 0
        : (DateTo - DateFrom).Days + 1;
}

[ProtoContract]
public class InstrumentSettings
{
    public InstrumentSettings()
    {
    }

    public InstrumentSettings(InstrumentSettings other)
    {
        if (other is null) throw new ArgumentNullException(nameof(other));

        Ticker = other.Ticker;
        Description = other.Description;
        ContractSize = other.ContractSize;
        Units = other.Units;
        MinVolume = other.MinVolume;
        MaxVolume = other.MaxVolume;
        VolumeStep = other.VolumeStep;
        MinTick = other.MinTick;
        Leverage = other.Leverage;
        TradeMode = other.TradeMode;
        Category = other.Category;
        
        Summary = new SymbolDataSummary(other.Summary);
    }

    [JsonPropertyName("ticker")]
    [ProtoMember(1)]
    public string Ticker { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    [ProtoMember(2)]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("contract_size")]
    [ProtoMember(3)]
    public double ContractSize { get; set; } = 1.0;

    [JsonPropertyName("units")]
    [ProtoMember(4)]
    public string Units { get; set; } = string.Empty;

    [JsonPropertyName("min_volume")]
    [ProtoMember(5)]
    public double MinVolume { get; set; } = 1.0;

    [JsonPropertyName("max_volume")]
    [ProtoMember(6)]
    public double MaxVolume { get; set; } = 100.0;

    [JsonPropertyName("volume_step")]
    [ProtoMember(7)]
    public double VolumeStep { get; set; } = 0.1;

    [JsonPropertyName("min_tick")]
    [ProtoMember(8)]
    public double MinTick { get; set; } = 0.001;

    [JsonPropertyName("leverage")]
    [ProtoMember(9)]
    public double Leverage { get; set; } = 0.05;

    [JsonPropertyName("trade_mode")]
    [ProtoMember(10)]
    public TradeMode TradeMode { get; set; } = TradeMode.Full;

    [JsonIgnore] [ProtoMember(11)] public string Category { get; set; } = string.Empty;
    [JsonIgnore] [ProtoIgnore] public SymbolDataSummary Summary { get; set; } = new SymbolDataSummary();

    public static Comparison<InstrumentSettings> GetComparer(string prop, bool desc)
    {
        Comparison<InstrumentSettings>? cmp = prop switch
        {
            nameof(Ticker) => (a, b) =>
                string.Compare(a.Ticker, b.Ticker, StringComparison.OrdinalIgnoreCase),
            nameof(Description) => (a, b) =>
                string.Compare(a.Description, b.Description, StringComparison.OrdinalIgnoreCase),
            nameof(Summary.MinimalTimeframe) => (a, b) =>
                a.Summary.MinimalTimeframe.CompareTo(b.Summary.MinimalTimeframe),
            nameof(Summary.DateFrom) => (a, b) => a.Summary.DateFrom.CompareTo(b.Summary.DateFrom),
            nameof(Summary.DateTo) => (a, b) => a.Summary.DateTo.CompareTo(b.Summary.DateTo),
            nameof(Summary.TotalDays) => (a, b) => a.Summary.TotalDays.CompareTo(b.Summary.TotalDays),
            nameof(Summary.TotalRecords) => (a, b) => a.Summary.TotalRecords.CompareTo(b.Summary.TotalRecords),
            nameof(Category) => (a, b) =>
                string.Compare(a.Category, b.Category, StringComparison.OrdinalIgnoreCase),
            _ => throw new ArgumentOutOfRangeException(nameof(prop))
        };

        if (desc)
        {
            var asc = cmp; // capture
            cmp = (a, b) => asc(b, a); // reverse
        }

        return cmp;
    }
}