using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeForge.BacktestEngine.Threading;

public sealed class TaskRunner<TResult> : IDisposable
{
    private readonly Func<Task<TResult>> _asyncWork;
    private readonly Action<TResult> _callback;
    private readonly CancellationTokenSource _cts = new();
    private Task? _runningTask = null;

    // Constructor for async work
    public TaskRunner(Func<Task<TResult>> asyncWork, Action<TResult> callback)
    {
        _asyncWork = asyncWork ?? throw new ArgumentNullException(nameof(asyncWork));
        _callback = callback ?? throw new ArgumentNullException(nameof(callback));
    }

    // Constructor for sync work (optional, for backward compatibility)
    public TaskRunner(Func<TResult> syncWork, Action<TResult> callback)
        : this(() => Task.FromResult(syncWork()), callback)
    {
    }

    ~TaskRunner()
    {
        Dispose(false);
    }

    public bool IsRunning => _runningTask != null;

    public void Cancel() => _cts.Cancel();

    public Task Start()
    {
        if (_runningTask != null)
            throw new InvalidOperationException("TaskRunner<TResult> уже запущен.");

        _runningTask = RunAsync(_cts.Token);
        return _runningTask;
    }

    private async Task RunAsync(CancellationToken token)
    {
        try
        {
            var result = await _asyncWork().ConfigureAwait(false);
            token.ThrowIfCancellationRequested();
            _callback(result);
        }
        catch (OperationCanceledException) { }
        catch
        {
            throw;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _runningTask?.Wait();
            }
            _cts.Dispose();
        }
    }
}
public sealed class TaskRunner(Action work, Action? callback = null) : IDisposable
{
    private readonly Action _work = work ?? throw new ArgumentNullException(nameof(work));

    private readonly CancellationTokenSource _cts = new();
    private Task? _runningTask = null;

    ~TaskRunner()
    {
        
    }

    public void Cancel() => _cts.Cancel();
    public bool IsRunning => _runningTask != null;

    public void Dispose()
    {
        if (_cts.IsCancellationRequested) return;

        _cts.Cancel();
        _runningTask?.Wait();
        _cts.Dispose();
        GC.SuppressFinalize(this);
    }

    public Task Start()
    {
        if (_runningTask != null)
            throw new InvalidOperationException("TaskRunner уже запущен.");

        _runningTask = RunAsync(_cts.Token);
        return _runningTask;
    }

    private async Task RunAsync(CancellationToken token)
    {
        try
        {
            await Task.Run(_work, token);
            token.ThrowIfCancellationRequested();
            callback?.Invoke();
        }
        catch (OperationCanceledException) {  }
        catch
        {
            throw;
        }
    }
}
