using System.Globalization;
using TradeForge.BacktestEngine.Models;
using TradeForge.BacktestEngine.Threading;
using TradeForge.Core.Extensions;
using TradeForge.Core.Models;

namespace TradeForge.BacktestEngine.Services;

public class BacktestEngine : IDisposable
{
    public Account? BacktestAccount { get; protected set; } = null;
    public BacktestInitParams? InitParams { get; protected set; } = null;
    protected TaskRunner<BacktestResult>? TaskRunner { get; set; } = null;
    protected bool IsRunning { get; set; } = false;
    public event Action<BacktestError>? Faulted;
    public event Action? Canceled;

    private Guid? _backtestId = null;

    public Guid BacktestId
    {
        get
        {
            _backtestId ??= Guid.NewGuid();
            return _backtestId.Value;
        }
    }

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
            {
                throw new ArgumentException("Data is empty");
            }

            if (initParams.Instrument is null)
            {
                throw new ArgumentException("Instrument is empty");
            }

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
        }
    }

    public void Cancel()
    {
        if (IsRunning)
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

    protected void TaskRunnerCallback(BacktestResult result)
    {
        IsRunning = false;
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

    public void Dispose()
    {
        TaskRunner?.Cancel();
        TaskRunner?.Dispose();
        TaskRunner = null;
        IsRunning = false;
    }


    protected virtual void OnFaulted(Exception ex)
    {
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
        Canceled?.Invoke();
    }
}