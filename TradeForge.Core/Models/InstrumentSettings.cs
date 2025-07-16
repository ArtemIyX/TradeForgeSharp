using TradeForge.Core.Enums;
using TradeForge.Core.Extensions;

namespace TradeForge.Core.Models;

using System.Text.Json.Serialization;

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
public record  InstrumentSettings
{
    [JsonPropertyName("ticker")]
    public string Ticker { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("contract_size")]
    public decimal ContractSize { get; set; }

    [JsonPropertyName("units")]
    public string Units { get; set; } = string.Empty;

    [JsonPropertyName("min_volume")]
    public decimal MinVolume { get; set; }

    [JsonPropertyName("max_volume")]
    public decimal MaxVolume { get; set; }

    [JsonPropertyName("volume_step")]
    public decimal VolumeStep { get; set; }

    [JsonPropertyName("min_tick")]
    public decimal MinTick { get; set; }

    [JsonPropertyName("leverage")]
    public decimal Leverage { get; set; }
    
    [JsonPropertyName("trade_mode")]
    public TradeMode TradeMode { get; set; }
}