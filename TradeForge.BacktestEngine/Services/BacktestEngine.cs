namespace TradeForge.BacktestEngine.Services;

using TradeForge.BacktestEngine.Models;
using TradeForge.Core.Enums.Framework;
using TradeForge.Core.Models;
using TradeForge.Core.Models.Framework;

public class BacktestEngine : IDisposable, IAsyncDisposable
{
    public Account? BacktestAccount { get; protected set; } = null;
    public BacktestInitParams? InitParams { get; protected set; } = null;
    protected bool IsRunning { get; set; } = false;

    public event Action<BacktestError>? Faulted;
    public event Action? Canceled;
    public event Action<BacktestResult>? Finished;

    private Guid? _backtestId = null;

    private readonly TaskCompletionSource<BacktestResult?> _tcsCompletion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    // Trading state
    private readonly List<Order> _orders = new();
    private readonly List<Position> _positions = new();
    private readonly List<Deal> _deals = new();

    private double _currentPrice = 0.0;
    private DateTime _currentTime = DateTime.MinValue;
    private double _maxEquity = 0.0;
    private double _maxDrawdown = 0.0;

    public Guid BacktestId
    {
        get
        {
            _backtestId ??= Guid.NewGuid();
            return _backtestId.Value;
        }
    }

    #region MQL5-Style Trading Functions

    /// <summary>
    /// Send market or pending order (like OrderSend in MQL5)
    /// </summary>
    public TradeResult OrderSend(TradeRequest request)
    {
        try
        {
            if (BacktestAccount == null || InitParams == null)
                return new TradeResult { Success = false, Message = "Engine not initialized" };

            var order = new Order
            {
                AccountId = BacktestAccount.AccountId,
                TimeSetup = _currentTime,
                Type = request.Type,
                Symbol = request.Symbol,
                Volume = request.Volume,
                PriceOpen = request.Price ?? _currentPrice,
                StopLoss = request.StopLoss,
                TakeProfit = request.TakeProfit,
                Comment = request.Comment,
                MagicNumber = request.MagicNumber
            };

            // Market orders execute immediately
            if (request.Type == OrderType.Buy || request.Type == OrderType.Sell)
            {
                return ExecuteMarketOrder(order);
            }
            else // Pending orders
            {
                order.State = OrderState.Pending;
                _orders.Add(order);
                return new TradeResult
                {
                    Success = true,
                    Message = "Pending order placed",
                    OrderId = order.OrderId
                };
            }
        }
        catch (Exception ex)
        {
            return new TradeResult { Success = false, Message = ex.Message };
        }
    }

    /// <summary>
    /// Close position by position ID (like PositionClose in MQL5)
    /// </summary>
    public TradeResult PositionClose(string positionId)
    {
        try
        {
            var position = _positions.FirstOrDefault(p => p.PositionId == positionId);
            if (position == null)
                return new TradeResult { Success = false, Message = "Position not found" };

            return ClosePosition(position, _currentPrice, DealReason.Expert);
        }
        catch (Exception ex)
        {
            return new TradeResult { Success = false, Message = ex.Message };
        }
    }

    /// <summary>
    /// Close position by symbol (like PositionCloseBy in MQL5)
    /// </summary>
    public TradeResult PositionCloseBySymbol(string symbol)
    {
        try
        {
            var position = _positions.FirstOrDefault(p => p.Symbol == symbol);
            if (position == null)
                return new TradeResult { Success = false, Message = "No position for this symbol" };

            return ClosePosition(position, _currentPrice, DealReason.Expert);
        }
        catch (Exception ex)
        {
            return new TradeResult { Success = false, Message = ex.Message };
        }
    }

    /// <summary>
    /// Modify position SL/TP (like PositionModify in MQL5)
    /// </summary>
    public TradeResult PositionModify(string positionId, double? stopLoss, double? takeProfit)
    {
        try
        {
            var position = _positions.FirstOrDefault(p => p.PositionId == positionId);
            if (position == null)
                return new TradeResult { Success = false, Message = "Position not found" };

            position.StopLoss = stopLoss;
            position.TakeProfit = takeProfit;

            return new TradeResult { Success = true, Message = "Position modified" };
        }
        catch (Exception ex)
        {
            return new TradeResult { Success = false, Message = ex.Message };
        }
    }

    /// <summary>
    /// Delete pending order (like OrderDelete in MQL5)
    /// </summary>
    public TradeResult OrderDelete(string orderId)
    {
        try
        {
            var order = _orders.FirstOrDefault(o => o.OrderId == orderId && o.State == OrderState.Pending);
            if (order == null)
                return new TradeResult { Success = false, Message = "Pending order not found" };

            order.State = OrderState.Canceled;
            order.TimeCanceled = _currentTime;

            return new TradeResult { Success = true, Message = "Order canceled" };
        }
        catch (Exception ex)
        {
            return new TradeResult { Success = false, Message = ex.Message };
        }
    }

    /// <summary>
    /// Get all open positions
    /// </summary>
    public List<Position> GetPositions() => new List<Position>(_positions);

    /// <summary>
    /// Get pending orders
    /// </summary>
    public List<Order> GetPendingOrders() => _orders.Where(o => o.State == OrderState.Pending).ToList();

    /// <summary>
    /// Get position by symbol
    /// </summary>
    public Position? GetPositionBySymbol(string symbol) => _positions.FirstOrDefault(p => p.Symbol == symbol);

    #endregion

    #region Internal Trading Logic

    private TradeResult ExecuteMarketOrder(Order order)
    {
        if (BacktestAccount == null || InitParams == null)
            return new TradeResult { Success = false, Message = "Engine not initialized" };

        // Apply slippage and spread
        double executionPrice = CalculateExecutionPrice(order.Type, _currentPrice);

        // Check margin requirements
        double requiredMargin = CalculateRequiredMargin(order.Volume, executionPrice);
        if (BacktestAccount.Equity < requiredMargin)
            return new TradeResult { Success = false, Message = "Insufficient margin" };

        // Create position
        var position = new Position
        {
            AccountId = BacktestAccount.AccountId,
            Symbol = order.Symbol,
            TimeOpen = _currentTime,
            Type = order.Type == OrderType.Buy ? DealType.Buy : DealType.Sell,
            Volume = order.Volume,
            PriceOpen = executionPrice,
            StopLoss = order.StopLoss,
            TakeProfit = order.TakeProfit,
            Comment = order.Comment,
            MagicNumber = order.MagicNumber,
            OpeningOrderId = order.OrderId
        };

        // Apply commission
        position.Commission = CalculateCommission(order.Volume);
        BacktestAccount.Balance -= position.Commission;

        // Create opening deal
        var deal = new Deal
        {
            PositionId = position.PositionId,
            OrderId = order.OrderId,
            AccountId = BacktestAccount.AccountId,
            Time = _currentTime,
            Type = position.Type,
            Reason = DealReason.Expert,
            Symbol = order.Symbol,
            Volume = order.Volume,
            Price = executionPrice,
            Commission = position.Commission,
            Comment = order.Comment,
            MagicNumber = order.MagicNumber
        };

        // Update order state
        order.State = OrderState.Filled;
        order.TimeFilled = _currentTime;
        order.PositionId = position.PositionId;

        _positions.Add(position);
        _deals.Add(deal);

        UpdateEquity();

        return new TradeResult
        {
            Success = true,
            Message = "Order executed",
            OrderId = order.OrderId,
            PositionId = position.PositionId,
            DealId = deal.DealId,
            ExecutionPrice = executionPrice
        };
    }

    private TradeResult ClosePosition(Position position, double closePrice, DealReason reason)
    {
        if (BacktestAccount == null || InitParams == null)
            return new TradeResult { Success = false, Message = "Engine not initialized" };

        // Apply slippage for closing
        double executionPrice = CalculateExecutionPrice(
            position.Type == DealType.Buy ? OrderType.Sell : OrderType.Buy,
            closePrice
        );

        // Calculate profit
        double profit = CalculateProfit(position, executionPrice);
        position.Profit = profit;
        position.PriceClose = executionPrice;
        position.TimeClose = _currentTime;

        // Create closing deal
        var closeDeal = new Deal
        {
            PositionId = position.PositionId,
            OrderId = Guid.NewGuid().ToString(),
            AccountId = BacktestAccount.AccountId,
            Time = _currentTime,
            Type = position.Type == DealType.Buy ? DealType.Sell : DealType.Buy,
            Reason = reason,
            Symbol = position.Symbol,
            Volume = position.Volume,
            Price = executionPrice,
            Profit = profit,
            Swap = position.Swap,
            Comment = position.Comment,
            MagicNumber = position.MagicNumber
        };

        // Update balance
        BacktestAccount.Balance += profit;

        _deals.Add(closeDeal);
        _positions.Remove(position);

        UpdateEquity();

        return new TradeResult
        {
            Success = true,
            Message = "Position closed",
            DealId = closeDeal.DealId,
            ExecutionPrice = executionPrice
        };
    }

    private void CheckPendingOrders(double open, double high, double low, double close)
    {
        var pendingOrders = _orders.Where(o => o.State == OrderState.Pending).ToList();

        foreach (var order in pendingOrders)
        {
            bool triggered = false;
            double triggerPrice = order.PriceOpen;

            switch (order.Type)
            {
                case OrderType.BuyLimit:
                    triggered = low <= triggerPrice;
                    break;
                case OrderType.SellLimit:
                    triggered = high >= triggerPrice;
                    break;
                case OrderType.BuyStop:
                    triggered = high >= triggerPrice;
                    break;
                case OrderType.SellStop:
                    triggered = low <= triggerPrice;
                    break;
            }

            if (triggered)
            {
                _currentPrice = triggerPrice;
                ExecuteMarketOrder(order);
            }
        }
    }

    private void CheckStopLossAndTakeProfit(double high, double low)
    {
        var positionsToClose = new List<(Position, double, DealReason)>();

        foreach (var position in _positions.ToList())
        {
            if (position.Type == DealType.Buy)
            {
                // Check Stop Loss
                if (position.StopLoss.HasValue && low <= position.StopLoss.Value)
                {
                    positionsToClose.Add((position, position.StopLoss.Value, DealReason.StopLoss));
                }
                // Check Take Profit
                else if (position.TakeProfit.HasValue && high >= position.TakeProfit.Value)
                {
                    positionsToClose.Add((position, position.TakeProfit.Value, DealReason.TakeProfit));
                }
            }
            else // Sell
            {
                // Check Stop Loss
                if (position.StopLoss.HasValue && high >= position.StopLoss.Value)
                {
                    positionsToClose.Add((position, position.StopLoss.Value, DealReason.StopLoss));
                }
                // Check Take Profit
                else if (position.TakeProfit.HasValue && low <= position.TakeProfit.Value)
                {
                    positionsToClose.Add((position, position.TakeProfit.Value, DealReason.TakeProfit));
                }
            }
        }

        foreach (var (position, price, reason) in positionsToClose)
        {
            ClosePosition(position, price, reason);
        }
    }

    private void UpdateEquity()
    {
        if (BacktestAccount == null || InitParams == null) return;

        double floatingPL = 0.0;
        foreach (var position in _positions)
        {
            floatingPL += CalculateProfit(position, _currentPrice);
        }

        BacktestAccount.Equity = BacktestAccount.Balance + floatingPL;

        // Track drawdown
        if (BacktestAccount.Equity > _maxEquity)
            _maxEquity = BacktestAccount.Equity;

        double currentDrawdown = _maxEquity - BacktestAccount.Equity;
        if (currentDrawdown > _maxDrawdown)
            _maxDrawdown = currentDrawdown;
    }

    private double CalculateProfit(Position position, double currentPrice)
    {
        if (InitParams?.Instrument == null) return 0.0;

        double priceDiff = position.Type == DealType.Buy
            ? (currentPrice - position.PriceOpen)
            : (position.PriceOpen - currentPrice);

        return priceDiff * position.Volume * InitParams.Instrument.ContractSize;
    }

    private double CalculateExecutionPrice(OrderType type, double basePrice)
    {
        if (InitParams == null) return basePrice;

        double spread = InitParams.Spread;
        double slippage = InitParams.Slippage;

        return type switch
        {
            OrderType.Buy or OrderType.BuyLimit or OrderType.BuyStop =>
                basePrice + spread + slippage,
            OrderType.Sell or OrderType.SellLimit or OrderType.SellStop =>
                basePrice - slippage,
            _ => basePrice
        };
    }

    private double CalculateRequiredMargin(double volume, double price)
    {
        if (InitParams?.Instrument == null) return 0.0;

        double leverage = InitParams.Leverage > 0 ? InitParams.Leverage : InitParams.Instrument.Leverage;
        return (volume * InitParams.Instrument.ContractSize * price) / leverage;
    }

    private double CalculateCommission(double volume)
    {
        // Simple commission model - can be customized
        return volume * 0.0; // No commission by default
    }

    private void CloseAllPositionsAndOrders()
    {
        // Cancel all pending orders
        foreach (var order in _orders.Where(o => o.State == OrderState.Pending).ToList())
        {
            order.State = OrderState.Canceled;
            order.TimeCanceled = _currentTime;
        }

        // Close all open positions
        foreach (var position in _positions.ToList())
        {
            ClosePosition(position, _currentPrice, DealReason.Expert);
        }
    }

    #endregion

    #region Backtest Execution

    public virtual void Run(BacktestInitParams initParams)
    {
        try
        {
            if (IsRunning)
                throw new Exception($"BacktestEngine is already running");

            InitParams = initParams;
            if (initParams.Data.Count <= 0)
                throw new ArgumentException("Data is empty");

            BacktestAccount = new Account()
            {
                Balance = InitParams.InitialBalance,
                Equity = InitParams.InitialBalance,
                AccountId = Guid.NewGuid().ToString(),
            };

            _maxEquity = InitParams.InitialBalance;

            initParams.Strategy.OnInit(this, BacktestAccount);

            IsRunning = true;
            Task.Run(async () => await TaskRunnerWork()).ContinueWith(t =>
            {
                if (t.IsFaulted)
                    OnFaulted(t.Exception!);
                else
                    TaskRunnerCallback(t.Result);
            });
        }
        catch (Exception ex)
        {
            OnFaulted(ex);
            _tcsCompletion.TrySetResult(null);
        }
    }

    protected virtual async Task<BacktestResult> TaskRunnerWork()
    {
        if (InitParams == null || BacktestAccount == null)
            throw new Exception("Engine not initialized");

        List<OHLC> data = InitParams.Data;
        BacktestStrategy strat = InitParams.Strategy;

        DateTime[] time = data.Select(x => x.Timestamp).ToArray();
        double[] open = data.Select(x => x.Open).ToArray();
        double[] high = data.Select(x => x.High).ToArray();
        double[] low = data.Select(x => x.Low).ToArray();
        double[] close = data.Select(x => x.Close).ToArray();

        for (int i = 0; i < data.Count; i++)
        {
            _currentTime = time[i];
            _currentPrice = close[i];

            // Check pending orders and SL/TP BEFORE OnBar
            CheckPendingOrders(open[i], high[i], low[i], close[i]);
            CheckStopLossAndTakeProfit(high[i], low[i]);

            // Call strategy
            await strat.OnBar(this, BacktestAccount, i, time, open, high, low, close);

            // Update equity at end of bar
            UpdateEquity();
        }

        // Close all remaining positions and orders
        CloseAllPositionsAndOrders();

        return GenerateBacktestResult();
    }

    private BacktestResult GenerateBacktestResult()
    {
        if (BacktestAccount == null || InitParams == null)
            return new BacktestResult();

        var closedDeals = _deals.Where(d => d.Reason != DealReason.Expert || d.Type != _deals.First().Type).ToList();

        var result = new BacktestResult
        {
            Deals = new List<Deal>(_deals),
            Orders = new List<Order>(_orders),
            InitialBalance = InitParams.InitialBalance,
            FinalBalance = BacktestAccount.Balance,
            FinalEquity = BacktestAccount.Equity,
            MaxDrawdown = _maxDrawdown,
            MaxDrawdownPercent = (_maxDrawdown / _maxEquity) * 100
        };

        // Calculate statistics
        var profitDeals = closedDeals.Where(d => d.Profit > 0).ToList();
        var lossDeals = closedDeals.Where(d => d.Profit < 0).ToList();

        result.TotalProfit = profitDeals.Sum(d => d.Profit);
        result.TotalLoss = Math.Abs(lossDeals.Sum(d => d.Profit));
        result.NetProfit = result.FinalBalance - result.InitialBalance;

        result.TotalTrades = closedDeals.Count / 2; // Entry + Exit = 1 trade
        result.WinningTrades = profitDeals.Count;
        result.LosingTrades = lossDeals.Count;

        result.WinRate = result.TotalTrades > 0 ? (double)result.WinningTrades / result.TotalTrades * 100 : 0;
        result.ProfitFactor = result.TotalLoss > 0 ? result.TotalProfit / result.TotalLoss : 0;

        if (InitParams.Data.Count > 0)
        {
            result.StartDate = InitParams.Data.First().Timestamp;
            result.EndDate = InitParams.Data.Last().Timestamp;
        }

        return result;
    }

    protected virtual void TaskRunnerCallback(BacktestResult result)
    {
        IsRunning = false;
        _tcsCompletion.TrySetResult(result);
        OnFinished(result);
    }

    public virtual async Task<BacktestResult?> AwaitCompletionAsync(CancellationToken token = default)
    {
        if (_tcsCompletion.Task.IsCompleted)
            return await _tcsCompletion.Task;

        var tcsToken = new TaskCompletionSource<bool>();
        await using (token.Register(() => tcsToken.TrySetResult(true)))
        {
            var completed = await Task.WhenAny(_tcsCompletion.Task, tcsToken.Task);
            if (completed == tcsToken.Task)
                throw new OperationCanceledException(token);

            return await _tcsCompletion.Task;
        }
    }

    #endregion

    #region Event Handlers

    protected virtual void OnFaulted(Exception ex)
    {
        IsRunning = false;
        _tcsCompletion.TrySetResult(null);

        CloseAllPositionsAndOrders();

        Faulted?.Invoke(new BacktestError()
        {
            Exception = ex,
            Snapshot = GenerateBacktestResult()
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

    #region Dispose

    public virtual void Dispose()
    {
        DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    public virtual async ValueTask DisposeAsync()
    {
        if (IsRunning)
        {
            IsRunning = false;
            await AwaitCompletionAsync();
        }
    }

    #endregion
}