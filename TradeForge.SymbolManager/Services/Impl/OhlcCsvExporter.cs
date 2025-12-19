using System.Globalization;
using CsvHelper;
using TradeForge.Core.Models;
using TradeForge.SymbolManager.Models;
using TradeForge.SymbolManager.Services.Interfaces;

namespace TradeForge.SymbolManager.Services.Impl;

public class OhlcCsvExporter : IOhlcCsvExporter
{
    public async Task ExportAsync(CsvExportRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.FilePath))
            throw new ArgumentException("FilePath is required.", nameof(request.FilePath));
        if (request.HeaderTemplate == null)
            throw new ArgumentException("HeaderTemplate is required.", nameof(request.HeaderTemplate));

        var records = request.OHLC ?? throw new ArgumentException("OHLC list cannot be null.", nameof(request.OHLC));
        int totalRecords = records.Count;

        // Ensure directory exists
        var directory = Path.GetDirectoryName(request.FilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await Task.Run(async () =>
        {
            await using var writer = new StreamWriter(request.FilePath, false); // false = do not append
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Register the custom class map (for column ordering, names, conversions, etc.)
            csv.Context.RegisterClassMap(request.HeaderTemplate);

            // Write header
            csv.WriteHeader<OHLC>();
            await csv.NextRecordAsync(); // Flush header

            // Progress setup
            request.Progress?.Report(0);

            int writtenRecords = 0;

            // Write records one by one with progress and cancellation support
            foreach (var record in records)
            {
                cancellationToken.ThrowIfCancellationRequested();

                csv.WriteRecord(record);
                await csv.NextRecordAsync();

                writtenRecords++;

                // Report progress as percentage (0-100)
                if (totalRecords > 0)
                {
                    int progress = (int)((double)writtenRecords / totalRecords * 100);
                    request.Progress?.Report(progress);
                }
            }

            // Ensure final flush and 100% progress
            await csv.FlushAsync();
            request.Progress?.Report(100);
        }, cancellationToken);
    }
}