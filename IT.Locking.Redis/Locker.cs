using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Locking.Redis;

public class Locker : Locking.Locker
{
    protected readonly IDatabase _db;
    protected readonly Func<Options?>? _getOptions;
    protected readonly Func<ReadOnlyMemory<Byte>> _newId;
    protected readonly ILogger? _logger;

    protected override Int32? RetryMin => _getOptions?.Invoke()?.RetryMin;

    protected override Int32? RetryMax => _getOptions?.Invoke()?.RetryMax;

    public Locker(IDatabase db,
        Func<Options?>? getOptions = null,
        Func<ReadOnlyMemory<Byte>>? newId = null,
        ILogger<Locker>? logger = null)
    {
        _db = db;
        _getOptions = getOptions;
        _newId = newId ?? (() => Guid.NewGuid().ToByteArray());
        _logger = logger;
    }

    #region IAsyncLocker

    public override async Task<IAsyncLocked?> TryAcquireAsync(String name, TimeSpan wait, CancellationToken cancellationToken)
    {
        var options = _getOptions?.Invoke();
        var prefix = options?.Prefix;
        var expiryMilliseconds = options?.Expiry;

        RedisKey key = prefix is null ? name : $"{prefix}:{name}";
        RedisValue value = _newId();
        var expiry = expiryMilliseconds.HasValue ? TimeSpan.FromMilliseconds(expiryMilliseconds.Value) : ExpiryDefault;
#if DEBUG
        if (Debugger.IsAttached) expiry = ExpiryDebug;
#endif
        if (await _db.StringSetAsync(key, value, expiry, when: When.NotExists).ConfigureAwait(false))
            return new Locked(_db, key, value);

        if (wait > TimeSpan.Zero)
        {
            var left = (Int32)wait.TotalMilliseconds;
            var min = options?.RetryMin ?? RetryMinDefault;
            var max = options?.RetryMax ?? RetryMaxDefault;
            var stopwatch = Stopwatch.StartNew();
            do
            {
                if (left < max) max = left;
                var retry = NextDelay(name, min, max);

                await Task.Delay(retry, cancellationToken).ConfigureAwait(false);

                if (await _db.StringSetAsync(key, value, expiry, when: When.NotExists).ConfigureAwait(false))
                    return new Locked(_db, key, value);

                var elapsed = stopwatch.Elapsed;

                if (elapsed > wait) return null;

                left = (Int32)(wait.TotalMilliseconds - elapsed.TotalMilliseconds);

                //Console.WriteLine($"{left} = {wait.TotalMilliseconds} - {elapsed.TotalMilliseconds}");
            } while (true);
        }

        return null;
    }

    #endregion IAsyncLocker

    #region ILocker

    public override ILocked? TryAcquire(String name, TimeSpan wait, CancellationToken cancellationToken)
    {
        var options = _getOptions?.Invoke();
        var prefix = options?.Prefix;
        var expiryMilliseconds = options?.Expiry;

        RedisKey key = prefix is null ? name : $"{prefix}:{name}";
        RedisValue value = _newId();
        var expiry = expiryMilliseconds.HasValue ? TimeSpan.FromMilliseconds(expiryMilliseconds.Value) : ExpiryDefault;
#if DEBUG
        if (Debugger.IsAttached) expiry = ExpiryDebug;
#endif
        if (_db.StringSet(key, value, expiry, when: When.NotExists))
            return new Locked(_db, key, value);

        if (wait > TimeSpan.Zero)
        {
            var left = (Int32)wait.TotalMilliseconds;
            var min = options?.RetryMin ?? RetryMinDefault;
            var max = options?.RetryMax ?? RetryMaxDefault;
            var stopwatch = Stopwatch.StartNew();
            do
            {
                if (left < max) max = left;
                var retry = NextDelay(name, min, max);

                Task.Delay(retry, cancellationToken).Wait(cancellationToken);

                if (_db.StringSet(key, value, expiry, when: When.NotExists))
                    return new Locked(_db, key, value);

                var elapsed = stopwatch.Elapsed;

                if (elapsed > wait) return null;

                left = (Int32)(wait.TotalMilliseconds - elapsed.TotalMilliseconds);

                //Console.WriteLine($"{left} = {wait.TotalMilliseconds} - {elapsed.TotalMilliseconds}");
            } while (true);
        }

        return null;
    }

    #endregion ILocker

    protected override Int32 NextDelay(String name, Int32 min, Int32 max)
    {
        if (max <= min)
        {
#if DEBUG
            if (_logger == null)
                Debug.WriteLine($"Lock '{name}' thread '{Thread.CurrentThread.ManagedThreadId}' delay {max}ms");
#endif
            if (_logger != null && _logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug($"Lock '{name}' thread '{Thread.CurrentThread.ManagedThreadId}' delay {max}ms");

            return max;
        }

        var delay = GetRandom().Next(min, max);

#if DEBUG
        if (_logger == null)
            Debug.WriteLine($"Lock '{name}' thread '{Thread.CurrentThread.ManagedThreadId}' random delay {delay}ms (from {min} to {max})");
#endif
        if (_logger != null && _logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug($"Lock '{name}' thread '{Thread.CurrentThread.ManagedThreadId}' random delay {delay}ms (from {min} to {max})");

        return delay;
    }
}