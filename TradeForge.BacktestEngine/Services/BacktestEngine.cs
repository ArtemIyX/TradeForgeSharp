using System.Globalization;
using TradeForge.BacktestEngine.Models;
using TradeForge.BacktestEngine.Threading;
using TradeForge.Core.Models;

namespace TradeForge.BacktestEngine.Services;

public class BacktestEngine : IDisposable
{
    public Account? BacktestAccount { get; protected set; } = null;

    public BacktestInitParams? InitParams { get; protected set; } = null;

    protected TaskRunner<BacktestResult>? TaskRunner { get; set; } = null;

    protected bool IsRunning { get; set; } = false;
    
    public void Run(BacktestInitParams initParams)
    {
        if (IsRunning)
        {
            throw new Exception($"BacktestEngine (Strategy '{InitParams?.Strategy.ToString()}') is arleady running");
        }
        

        InitParams = initParams;
        BacktestAccount = new Account()
        {
            Balance = InitParams.InitialBalance,
            Equity = InitParams.InitialBalance,
            AccountId = Guid.NewGuid().ToString(),
        };
        TaskRunner = new TaskRunner<BacktestResult>(TaskRunnerWork, TaskRunnerCallback);
    }

    protected void TaskRunnerCallback(BacktestResult result)
    {
        
    }

    protected async Task<BacktestResult> TaskRunnerWork()
    {
        var backtestResult = new BacktestResult();

        for (int i = 0; i < InitParams.Data.Count; i++)
        {
            
        }
        
        return backtestResult;
    }

    public void Dispose()
    {
        TaskRunner?.Cancel();
        TaskRunner?.Dispose();
    }
}