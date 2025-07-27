using System.Globalization;
using System.Text.Json;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TradeForge.BacktestEngine.Models;
using TradeForge.BacktestEngine.Services;
using TradeForge.Core.Enums;
using TradeForge.Core.Models;
using TradeForge.SymbolManager.Models;
using TradeForge.SymbolManager.Services.Impl;
using TradeForge.SymbolManager.Services.Interfaces;

namespace ConsoleBacktester
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((ctx, services) =>
                {
                    services.AddSingleton<IOhlcCsvImporter, OhlcCsvImporter>();
                    services.AddHostedService<BacktestWorker>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}