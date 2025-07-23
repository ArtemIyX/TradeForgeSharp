using Microsoft.AspNetCore.Components;
using TradeForge.Components.DataManager;
using TradeForge.Components.DataManager.Import;
using TradeForge.Components.Shared.Modals;
using TradeForge.Core.Models;
using TradeForge.Services;
using TradeForge.SymbolManager.Models;
using TradeForge.SymbolManager.Services.Interfaces;

namespace TradeForge.Components.Pages;

public partial class DataManagerPage : ComponentBase, IDisposable
{
    private string ActiveTab = "symbols";

    public ConfirmationModal DeleteSymbolModal { get; set; }
    public ImportCSVFooter? ImportCSVFooter { get; set; }
    public ImportCSVDialog? ImportCSVDialog { get; set; }
    public SymbolTable SymbolTableRef { get; set; }
    public ChildModal ImportCSVModal { get; set; }

    public GenericPromptModal RenameSymbolModal { get; set; }

    private void SetTab(string tab)
    {
        ActiveTab = tab;
        switch (ActiveTab)
        {
            case "symbols":

                break;
        }
    }

    private InstrumentSettings? selectedSymbol = null;
    private InstrumentSettings? deleteSymbolChose = null;
    private string DeleteSymbolMessageStr => $"Are you sure you want delete symbol '{deleteSymbolChose?.Ticker}'?";

    [Inject] public ISymbolManager SymbolManager { get; set; }

    [Inject] public IAlertService Alert { get; set; }

    [Inject] public IOhlcCsvImporter ImporterService { get; set; }


    private bool _isImporting = false;
    
    protected override async Task OnInitializedAsync()
    {
    }

    protected void SymbolCreateRequest(string symbol)
    {
        try
        {
            SymbolManager.CreateSymbol(symbol);
            Alert.ShowInfo($"Creating '{symbol}' symbol...");

            RefreshSymbolTable();
        }
        catch (Exception ex)
        {
            Alert.ShowError($"Failed to create symbol: {ex.Message}");
        }
    }


    private void SymbolDeleteRequest(InstrumentSettings arg)
    {
        try
        {
            deleteSymbolChose = arg;
            DeleteSymbolModal.Show();
        }
        catch (Exception ex)
        {
            Alert.ShowError($"Failed to delete symbol: {ex.Message}");
        }
    }

    private void DeleteSymbolSuccess()
    {
        try
        {
            if (deleteSymbolChose is null)
            {
                throw new NullReferenceException("Delete chose is null");
            }

            string sym = deleteSymbolChose.Ticker;

            SymbolManager.DeleteSymbol(sym);

            Alert.ShowInfo($"Symbol '{sym}' has been deleted");
            deleteSymbolChose = null;

            RefreshSymbolTable();
        }
        catch (Exception ex)
        {
            Alert.ShowError($"Failed to delete symbol: {ex.Message}");
        }
    }

    private InstrumentSettings? _importSymbolSelect = null;

    private async Task SymbolImportRequest(InstrumentSettings symbol)
    {
        _importSymbolSelect = symbol;
        ImportCSVModal.Show();
    }

    private void OnImportDiscard()
    {
        if (_isImporting && _importCancellation != null)
        {
            _isImporting = false;
            _importCancellation?.Cancel();
            Alert.ShowWarning("Import cancelled");
        }

        ImportCSVModal?.Close();
    }


    private CancellationTokenSource? _importCancellation;

    private async Task OnImport()
    {
        if (_isImporting)
            return;

        InstrumentSettings? symbolInStorage =
            _importSymbolSelect ?? SymbolManager.GetSymbol(_importSymbolSelect?.Ticker ?? "");
        if (symbolInStorage is null)
        {
            ImportCSVModal.Close();
            Alert.ShowError("Failed to found symbol in storage");
            RefreshSymbolTable();
            return;
        }

        // Reset
        ImportCSVFooter.ErrorMessage = null;
        ImportCSVFooter.SetLoading(true);
        ImportCSVFooter.Progress = 0;
        _isImporting = true;
        _importCancellation = new CancellationTokenSource();

        try
        {
            if (string.IsNullOrEmpty(ImportCSVDialog.FilePath))
            {
                throw new Exception("Please specify a file path");
            }

            var progress = new Progress<int>(OnImportProgressReport);
            var request = new CsvImportRequest()
            {
                FilePath = ImportCSVDialog.FilePath,
                HeaderTemplate = ImportCSVDialog.BuildClassMap(),
                Progress = progress
            };
            IReadOnlyList<OHLC> ohlc = await Task.Run(() =>
                    ImporterService.ImportAsync(request, _importCancellation.Token).Result,
                _importCancellation.Token);

            await Task.Run(() => { SymbolManager.ImportData(symbolInStorage.Ticker, ohlc.ToList()); });

            ImportCSVFooter.SetLoading(false);
            ImportCSVModal.Close();

            Alert.ShowInfo($"Imported {ohlc.Count} rows for '{symbolInStorage.Ticker}'");
        }
        catch (Exception ex)
        {
            ImportCSVFooter.ErrorMessage = $"Error: {ex.Message}";
            ImportCSVFooter.SetLoading(false);
        }
        finally
        {
            ImportCSVFooter.Progress = 0;
            _isImporting = false;
        }
    }

    private void OnImportProgressReport(int value)
    {
        try
        {
            ImportCSVFooter.Progress = value;
            InvokeAsync(async () =>
            {
                StateHasChanged();
                await Task.Delay(1); // throttle
            });
        }
        catch (Exception e)
        {
            Alert.ShowError($"Failed report progress: {e.Message}");
        }
    }


    public void Dispose()
    {
        _importCancellation?.Dispose();
    }

    private InstrumentSettings? _renameSymbolRequest = null;

    private async Task SymbolRenameRequest(InstrumentSettings symbol)
    {
        _renameSymbolRequest = symbol;
        RenameSymbolModal.Show();
    }

    private void RenameSymbolAccept(string newSymbolName)
    {
        try
        {
            SymbolManager.RenameSymbol(_renameSymbolRequest.Ticker, newSymbolName);
            RefreshSymbolTable();
            Alert.ShowInfo($"Renamed '{_renameSymbolRequest.Ticker}' to '{newSymbolName}'");
        }
        catch (Exception ex)
        {
            Alert.ShowError($"Failed to rename symbol: {ex.Message}");
        }
    }

    private void RefreshSymbolTable()
    {
        SymbolTableRef.RefreshSymbols();
        selectedSymbol = null;
        InvokeAsync(StateHasChanged);
    }

    private void OnSymbolTableRefreshed()
    {
        selectedSymbol = null;
        InvokeAsync(StateHasChanged);
    }
}