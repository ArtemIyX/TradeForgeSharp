using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeForge.BacktestEngine.Threading;

public sealed class TaskRunner<TResult> : IDisposable
{
    private readonly Func<TResult> _work;
    private readonly Action<TResult> _callback;
    private readonly CancellationTokenSource _cts = new();
    private Task? _runningTask = null;

    public TaskRunner(Func<TResult> work, Action<TResult> callback)
    {
        _work = work ?? throw new ArgumentNullException(nameof(work));
        _callback = callback ?? throw new ArgumentNullException(nameof(callback));
    }

    public void Cancel() => _cts.Cancel();

    public void Start()
    {
        if (_runningTask != null)
            throw new InvalidOperationException("TaskRunner<TResult> уже запущен.");

        _runningTask = RunAsync(_cts.Token);
    }

    private async Task RunAsync(CancellationToken token)
    {
        try
        {
            var result = await Task.Run(_work, token);
            token.ThrowIfCancellationRequested();
            _callback(result);
        }
        catch (OperationCanceledException) {  }
        catch
        {
            throw;
        }
    }

    public void Dispose()
    {
        if (_cts.IsCancellationRequested) return;

        _cts.Cancel();
        _runningTask?.Wait();
        _cts.Dispose();
        GC.SuppressFinalize(this);
    }
}
public sealed class TaskRunner : IDisposable
{
    private readonly Action _work;
    private readonly Action _callback;

    private readonly CancellationTokenSource _cts = new();
    private Task? _runningTask = null;

    public TaskRunner(Action work, Action callback = null)
    {
        _work = work ?? throw new ArgumentNullException(nameof(work));
        _callback = callback;
    }

    public void Cancel() => _cts.Cancel();

    public void Dispose()
    {
        if (_cts.IsCancellationRequested) return;

        _cts.Cancel();
        _runningTask?.Wait();
        _cts.Dispose();
        GC.SuppressFinalize(this);
    }

    public void Start()
    {
        if (_runningTask != null)
            throw new InvalidOperationException("TaskRunner уже запущен.");

        _runningTask = RunAsync(_cts.Token);
    }

    private async Task RunAsync(CancellationToken token)
    {
        try
        {
            await Task.Run(_work, token);
            token.ThrowIfCancellationRequested();
            _callback?.Invoke();
        }
        catch (OperationCanceledException) {  }
        catch
        {
            throw;
        }
    }
}
