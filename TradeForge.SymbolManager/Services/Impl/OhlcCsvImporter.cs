using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using TradeForge.Core.Models;
using TradeForge.SymbolManager.Models;
using TradeForge.SymbolManager.Services.Interfaces;

namespace TradeForge.SymbolManager.Services.Impl;

public sealed class OhlcCsvImporter : IOhlcCsvImporter
{
    public async Task<IReadOnlyList<OHLC>> ImportAsync(
        CsvImportRequest request,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(async () =>
        {
            if (!File.Exists(request.FilePath))
                throw new FileNotFoundException(request.FilePath);

            var map = request.HeaderTemplate;
            using var reader = new StreamReader(request.FilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap(map);

            var rows = new List<OHLC>(1024);
            var length = new FileInfo(request.FilePath).Length;
            long bytesRead = 0;

            while (await csv.ReadAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var record = csv.GetRecord<OHLC>();
                rows.Add(record);

                bytesRead = reader.BaseStream.Position;
                request.Progress?.Report((int)(bytesRead * 100 / length));

                //await Task.Delay(TimeSpan.FromMilliseconds(25), cancellationToken);
            }

            request.Progress?.Report(100);
            return rows;
        }, cancellationToken);
    }
}