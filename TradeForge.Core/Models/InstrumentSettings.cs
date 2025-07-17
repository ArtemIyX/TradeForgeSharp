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
[ProtoContract]
public class InstrumentSettings
{
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

    [JsonIgnore] [ProtoMember(11)] public Timeframe MinimalTimeframe { get; set; } = Timeframe.M1;
    [JsonIgnore] [ProtoMember(12)] public DateTime DateFrom { get; set; } = DateTime.MinValue;
    [JsonIgnore] [ProtoMember(13)] public DateTime DateTo { get; set; } = DateTime.MinValue;
    [JsonIgnore] [ProtoMember(14)] public int TotalRecords { get; set; }
    [JsonIgnore] [ProtoMember(15)] public string Category { get; set; } = string.Empty;

    [JsonIgnore] [ProtoIgnore] public int TotalDays => (DateTo - DateFrom).Days + 1;

    public static Comparison<InstrumentSettings> GetComparer(string prop, bool desc)
    {
        Comparison<InstrumentSettings>? cmp = prop switch
        {
            nameof(InstrumentSettings.Ticker) => (a, b) =>
                string.Compare(a.Ticker, b.Ticker, StringComparison.OrdinalIgnoreCase),
            nameof(InstrumentSettings.Description) => (a, b) =>
                string.Compare(a.Description, b.Description, StringComparison.OrdinalIgnoreCase),
            nameof(InstrumentSettings.MinimalTimeframe) => (a, b) => a.MinimalTimeframe.CompareTo(b.MinimalTimeframe),
            nameof(InstrumentSettings.DateFrom) => (a, b) => a.DateFrom.CompareTo(b.DateFrom),
            nameof(InstrumentSettings.DateTo) => (a, b) => a.DateTo.CompareTo(b.DateTo),
            nameof(InstrumentSettings.TotalDays) => (a, b) => a.TotalDays.CompareTo(b.TotalDays),
            nameof(InstrumentSettings.TotalRecords) => (a, b) => a.TotalRecords.CompareTo(b.TotalRecords),
            nameof(InstrumentSettings.Category) => (a, b) =>
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