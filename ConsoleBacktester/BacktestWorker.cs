using System.Globalization;
using System.Text.Json;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TradeForge.BacktestEngine.Models;
using TradeForge.BacktestEngine.Services;
using TradeForge.Core.Enums;
using TradeForge.Core.Models;
using TradeForge.SymbolManager.Models;
using TradeForge.SymbolManager.Services.Impl;
using TradeForge.SymbolManager.Services.Interfaces;

namespace ConsoleBacktester;

public sealed class BacktestWorker : BackgroundService
{
    private readonly ILogger<BacktestWorker> _log;
    private readonly IHostApplicationLifetime _life;
    private readonly IOhlcCsvImporter _csvImporter;

    public BacktestWorker(ILogger<BacktestWorker> log,
                          IHostApplicationLifetime life,
                          IOhlcCsvImporter csvImporter)
    {
        _log       = log;
        _life      = life;
        _csvImporter = csvImporter;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        try
        {
            await RunBacktestAsync(token);
        }
        catch (Exception ex)
        {
            _log.LogCritical(ex, "Unhandled exception during back-test");
        }
        finally
        {
            _life.StopApplication();   // tell the host we are done
        }
    }

    private async Task RunBacktestAsync(CancellationToken token)
    {
        try
        {
            _log.LogInformation("Starting back-test");
            
            // 1. Import data
            var dataPath = @"C:\Users\Wellsaik\Downloads\python_AAPL.csv";
            _log.LogInformation("Importing data from {DataPath}", dataPath);

            var ohlc = await _csvImporter.ImportAsync(new CsvImportRequest
            {
                HeaderTemplate = BuildClassMap(),
                FilePath = dataPath
            }, token);

            _log.LogInformation("Imported {RowCount} OHLC rows", ohlc.Count);
            
            // 2. Instrument settings
            var instrument = new InstrumentSettings
            {
                Ticker = "AAPL",
                Description = "Apple tech",
                ContractSize = 1.0,
                Units = "Share(s)",
                MinVolume = 1,
                MaxVolume = 100_000_000,
                VolumeStep = 1,
                MinTick = 0.01,
                Leverage = 1.0 / 20.0,
                TradeMode = TradeMode.Full,
                Category = "Stocks"
            };

            _log.LogInformation("Instrument settings: {Settings}", JsonSerializer.Serialize(instrument));
            
            // 3. Strategy
            var strategy = new SimpleStrategy();
            _log.LogInformation("Strategy: {StrategyName}", strategy);

            foreach (var p in strategy.Parameters)
                _log.LogInformation("Parameter: {Param}", JsonSerializer.Serialize(p));
            
            // 4. Run back-test
            await using var engine = new BacktestEngine();
            engine.Faulted += ex => _log.LogError(ex.Exception, "Back-test faulted");
            engine.Canceled += () => _log.LogInformation("Back-test canceled");
            engine.Finished += result =>
            {
                _log.LogInformation("Back-test finished with final balance {Balance}", result.FinalBalance);
                foreach (var d in result.Deals)
                    _log.LogInformation("Deal: {Deal}", JsonSerializer.Serialize(d));
            };

            engine.Run(new BacktestInitParams
            {
                Data = ohlc.ToList(),
                Instrument = instrument,
                InitialBalance = 10_000,
                Slippage = 0,
                Spread = 0,
                Strategy = strategy
            });
            
            // Wait until the engine signals completion
            await engine.AwaitCompletionAsync(token);
        }
        catch (Exception ex)
        {
            _log.LogCritical(ex, "Unhandled exception during back-test");
        }
    }

    // -----------------------------------------------------------------
    // CSV helper – unchanged except for logging
    // -----------------------------------------------------------------
    private sealed record FieldMap(string Label, string Header,
                                   Action<ClassMap<OHLC>, string> Apply);

    private sealed class DynamicOhlcMap : ClassMap<OHLC> { }

    private static ClassMap<OHLC> BuildClassMap()
    {
        var map = new DynamicOhlcMap();

        foreach (var f in _fields)
        {
            if (string.IsNullOrWhiteSpace(f.Header))
                throw new InvalidOperationException($"'{f.Label}' header cannot be empty");

            f.Apply(map, f.Header.Trim());
        }

        return map;
    }

    private static readonly List<FieldMap> _fields = new()
    {
        new("Timestamp", "Date",    (m, h) => m.Map(x => x.Timestamp).Name(h)),
        new("Open",      "Open",    (m, h) => m.Map(x => x.Open).Name(h)),
        new("High",      "High",    (m, h) => m.Map(x => x.High).Name(h)),
        new("Low",       "Low",     (m, h) => m.Map(x => x.Low).Name(h)),
        new("Close",     "Close",   (m, h) => m.Map(x => x.Close).Name(h)),
        new("Volume",    "Volume",  (m, h) => m.Map(x => x.Volume).Name(h))
    };
}