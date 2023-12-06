using Chaos.Common.Utilities;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Services.Other.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Other;

/// <summary>
///     Manages the item stock of merchants
/// </summary>
public sealed class StockService(ILogger<StockService> logger) : BackgroundService, IStockService
{
    private readonly DeltaTime DeltaTime = new();
    private readonly ILogger<StockService> Logger = logger;
    private readonly ConcurrentDictionary<string, MerchantStock> Stock = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public int GetStock(string key, string itemTemplateKey)
    {
        if (!InternalTryGetStock(key, itemTemplateKey, out var itemStock))
            return 0;

        return itemStock.CurrentStock;
    }

    /// <inheritdoc />
    public bool HasStock(string key, string itemTemplateKey)
    {
        if (!InternalTryGetStock(key, itemTemplateKey, out var itemStock))
            return false;

        return itemStock.HasStock;
    }

    /// <inheritdoc />
    public void RegisterStock(
        string key,
        IEnumerable<(string ItemTemplateKey, int MaxStock)> stock,
        TimeSpan restockInterval,
        int restockPercent)
    {
        var merchantStock = new MerchantStock(
            key,
            stock,
            restockInterval,
            restockPercent,
            Logger);

        var stockDic = merchantStock.Stock;

        Stock.AddOrUpdate(
            key,
            _ => merchantStock,
            (_, existing) =>
            {
                //for each existing item in stock, check if the new stock has it
                //if it does, update the current stock to the existing current stock
                //it is done this way so that if RegisterStock is called multiple times, currentStock will not reset
                foreach (var kvp in existing.Stock)
                    if (stockDic.TryGetValue(kvp.Key, out var newStock))
                        stockDic[kvp.Key] = new ItemStock(newStock.MaxStock, kvp.Value.CurrentStock);

                return merchantStock;
            });
    }

    /// <inheritdoc />
    public void Restock(string key, int percent)
    {
        if (!Stock.TryGetValue(key, out var merchantStock))
            return;

        merchantStock.Restock(percent);

        Logger.WithTopics(Topics.Entities.Merchant, Topics.Qualifiers.Forced, Topics.Actions.Update)
              .LogDebug("Manually restocked {@Key}", key);
    }

    /// <inheritdoc />
    public bool TryDecrementStock(string key, string itemTemplateKey, int amount = 1)
    {
        if (!InternalTryGetStock(key, itemTemplateKey, out var itemStock))
            return false;

        return itemStock.TryDecrementStock(amount);
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);

                var delta = DeltaTime.GetDelta;

                foreach (var kvp in Stock)
                    kvp.Value.Update(delta);
            } catch (OperationCanceledException)
            {
                //ignore
                return;
            }
    }

    private bool InternalTryGetStock(string key, string itemTemplateKey, [MaybeNullWhen(false)] out ItemStock itemStock)
    {
        itemStock = null;

        if (!Stock.TryGetValue(key, out var merchantStock))
            return false;

        return merchantStock.Stock.TryGetValue(itemTemplateKey, out itemStock);
    }

    internal sealed class ItemStock
    {
        private int _currentStock;
        internal int MaxStock { get; }

        internal int CurrentStock => _currentStock;

        internal bool HasStock => (MaxStock == -1) || (_currentStock > 0);

        internal ItemStock(int maxStock)
        {
            _currentStock = maxStock;
            MaxStock = maxStock;
        }

        internal ItemStock(int maxStock, int currentStock)
        {
            _currentStock = Math.Min(maxStock, currentStock);
            MaxStock = maxStock;
        }

        internal void Restock(int percent)
            => InterlockedEx.SetValue(
                ref _currentStock,
                () =>
                {
                    var res = _currentStock + MathEx.GetPercentOf<int>(MaxStock, percent);

                    return Math.Min(MaxStock, res);
                });

        internal bool TryDecrementStock(int amount)
        {
            if (MaxStock == -1)
                return true;

            var ret = false;

            InterlockedEx.SetValue(
                ref _currentStock,
                () =>
                {
                    if (_currentStock >= amount)
                    {
                        ret = true;

                        return _currentStock - amount;
                    }

                    ret = false;

                    return _currentStock;
                });

            return ret;
        }
    }

    internal sealed class MerchantStock : IDeltaUpdatable
    {
        private readonly string Key;
        private readonly ILogger Logger;
        private readonly TimeSpan RestockInterval;
        private readonly int RestockPct;
        private readonly IIntervalTimer RestockTimer;
        public ConcurrentDictionary<string, ItemStock> Stock { get; }

        internal MerchantStock(
            string key,
            IEnumerable<(string ItemTemplateKey, int MaxStock)> stock,
            TimeSpan restockInterval,
            int restockPct,
            ILogger logger)
        {
            Key = key;
            Logger = logger;

            Stock = new ConcurrentDictionary<string, ItemStock>(
                stock.Select(x => new KeyValuePair<string, ItemStock>(x.ItemTemplateKey, new ItemStock(x.MaxStock))),
                StringComparer.OrdinalIgnoreCase);

            RestockInterval = restockInterval;
            RestockPct = restockPct;
            RestockTimer = new IntervalTimer(RestockInterval, false);
            RestockTimer.SetOrigin(DateTime.UtcNow.Date);
        }

        /// <inheritdoc />
        public void Update(TimeSpan delta)
        {
            RestockTimer.Update(delta);

            if (RestockTimer.IntervalElapsed)
            {
                Restock(RestockPct);

                Logger.WithTopics(Topics.Entities.Merchant, Topics.Actions.Update)
                      .LogDebug("Auto restocked {@Key}", Key);
            }
        }

        internal void Restock(int percent)
        {
            foreach (var kvp in Stock)
                kvp.Value.Restock(percent);
        }
    }
}