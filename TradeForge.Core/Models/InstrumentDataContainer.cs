using System.Security.Cryptography.X509Certificates;
using ProtoBuf;
using TradeForge.Core.Enums;

namespace TradeForge.Core.Models;

[ProtoContract]
public class InstrumentDataContainer
{
    [ProtoMember(1)]
    public string Ticker { get; set; } = string.Empty;
    
    [ProtoMember(2)]
    public List<OHLC> OHLC { get; set; } = new List<OHLC>();
    

    public SymbolDataSummary Summary()
    {
        if (OHLC.Count == 0)
            return new SymbolDataSummary();
        
        //TODO: TimeFrames
        Timeframe minTf = Timeframe.D;
        DateTime minDate = OHLC.Min(b => b.Timestamp);
        DateTime maxDate = OHLC.Max(b => b.Timestamp);
        return new SymbolDataSummary
        {
            MinimalTimeframe = minTf,
            DateFrom         = minDate,
            DateTo           = maxDate,
            TotalRecords     = OHLC.Count
        };
    }
}