using System.Globalization;
using TradeForge.BacktestEngine.Models;
using TradeForge.BacktestEngine.Threading;
using TradeForge.Core.Extensions;
using TradeForge.Core.Models;

namespace TradeForge.BacktestEngine.Services;

public class BacktestEngine : IDisposable, IAsyncDisposable
{
    public Account? BacktestAccount { get; protected set; } = null;
    public BacktestInitParams? InitParams { get; protected set; } = null;
    protected TaskRunner<BacktestResult>? TaskRunner { get; set; } = null;
    protected bool IsRunning { get; set; } = false;
    public event Action<BacktestError>? Faulted;
    public event Action? Canceled;
    public event Action<BacktestResult>? Finished;

    private Guid? _backtestId = null;

    private readonly TaskCompletionSource<BacktestResult?> _tcsCompletion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    
    public Guid BacktestId
    {
        get
        {
            _backtestId ??= Guid.NewGuid();
            return _backtestId.Value;
        }
    }
   

    #region TaskRunner

    public void Run(BacktestInitParams initParams)
    {
        try
        {
            if (IsRunning)
            {
                throw new Exception(
                    $"BacktestEngine [{_backtestId.ToString()}] (Strategy '{InitParams?.Strategy.ToString()}') is arleady running");
            }

            InitParams = initParams;
            if (initParams.Data.Count <= 0)
                throw new ArgumentException("Data is empty");

            if (initParams.Instrument is null)
                throw new ArgumentException("Instrument is empty");

            BacktestAccount = new Account()
            {
                Balance = InitParams.InitialBalance,
                Equity = InitParams.InitialBalance,
                AccountId = Guid.NewGuid().ToString(),
            };

            initParams.Strategy.OnInit(this, BacktestAccount);

            IsRunning = true;
            TaskRunner = new TaskRunner<BacktestResult>(TaskRunnerWork, TaskRunnerCallback);
            TaskRunner.Start();
        }
        catch (Exception ex)
        {
            OnFaulted(ex);
            _tcsCompletion.TrySetResult(null); // never started
        }
    }

    public void Cancel()
    {
        if (!IsRunning)
        {
            return;
        }

        try
        {
            TaskRunner?.Cancel();
            TaskRunner?.Dispose();
            TaskRunner = null;
        }
        catch (Exception ex)
        {
            OnFaulted(ex);
        }
        finally
        {
            IsRunning = false;
            OnCanceled();
        }
    }


    protected async Task<BacktestResult> TaskRunnerWork()
    {
        if (InitParams is null)
        {
            throw new Exception("Init parameters are not set");
        }

        if (BacktestAccount is null)
        {
            throw new Exception("Backtest account is null");
        }

        List<OHLC> data = InitParams.Data;
        BacktestStrategy strat = InitParams.Strategy;

        DateTime[] time = data.Select(x => x.Timestamp).ToArray();
        double[] open = data.Select(x => x.Open).ToArray();
        double[] high = data.Select(x => x.Open).ToArray();
        double[] low = data.Select(x => x.Low).ToArray();
        double[] close = data.Select(x => x.Close).ToArray();

        BacktestResult backtestResult = new BacktestResult();

        for (int i = 0; i < data.Count; i++)
        {
            await strat.OnBar(this, BacktestAccount, i, time, open, high, low, close);
        }

        return backtestResult;
    }

    protected void TaskRunnerCallback(BacktestResult result)
    {
        IsRunning = false;
        _tcsCompletion.TrySetResult(result); // success
        OnFinished(result);
    }
    
     
    /// <summary>
    /// Returns a task that completes when the backtest has finished, been cancelled,
    /// or the supplied <paramref name="token"/> requests cancellation.
    /// Throws <see cref="OperationCanceledException"/> if <paramref name="token"/>
    /// fired before the backtest completed.
    /// </summary>
    public async Task<BacktestResult?> AwaitCompletionAsync(CancellationToken token = default)
    {
        // Fast-path: already finished
        if (_tcsCompletion.Task.IsCompleted)
            return await _tcsCompletion.Task;

        // Create a Task that completes when either the TCS or the token fires
        var tcsToken = new TaskCompletionSource<bool>();
        await using (token.Register(() => tcsToken.TrySetResult(true)))
        {
            var completed = await Task.WhenAny(_tcsCompletion.Task, tcsToken.Task);
            if (completed == tcsToken.Task)
                throw new OperationCanceledException(token);

            return await _tcsCompletion.Task; // will not throw here
        }
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        if (IsRunning)
        {
            Cancel(); // request graceful stop
            await AwaitCompletionAsync(); // wait until it really stopped
        }

        TaskRunner?.Dispose();
        TaskRunner = null;
    }

    #endregion

    #region Invokes

    protected virtual void OnFaulted(Exception ex)
    {
        IsRunning = false;
        _tcsCompletion.TrySetResult(null); // failure

        // BacktestAccount should be valid
        if (BacktestAccount is null)
        {
            Faulted?.Invoke(new BacktestError()
            {
                Exception = new Exception("Failed to get backtest account", ex),
                Snapshot = new BacktestResult()
            });
            return;
        }

        // TODO: Close all orders and deals to finalize balance
        // CloseAllPendingOrdersAndOpenDeals();

        List<Deal> clonedDeals = BacktestAccount.ClosedDeals.DeepCloneList();
        Faulted?.Invoke(new BacktestError()
        {
            Exception = ex,
            Snapshot = new BacktestResult()
            {
                Deals = clonedDeals,
                FinalBalance = BacktestAccount.Balance,
            }
        });
    }

    protected virtual void OnCanceled()
    {
        _tcsCompletion.TrySetResult(null);
        Canceled?.Invoke();
    }


    protected virtual void OnFinished(BacktestResult obj)
    {
        Finished?.Invoke(obj);
    }

    #endregion
}