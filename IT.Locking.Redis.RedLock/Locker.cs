using Microsoft.Extensions.Logging;
using RedLockNet;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Locking.Redis.RedLock;

public class Locker : Locking.Locker
{
    private readonly IDistributedLockFactory _factory;
    protected readonly Func<Options?>? _getOptions;
    protected readonly ILogger? _logger;

    protected override Int32? RetryMin => _getOptions?.Invoke()?.RetryMin;

    protected override Int32? RetryMax => _getOptions?.Invoke()?.RetryMax;

    public Locker(IDistributedLockFactory factory, Func<Options?>? getOptions = null, ILogger<Locker>? logger = null)
    {
        _factory = factory;
        _getOptions = getOptions;
        _logger = logger;
    }

    #region IAsyncLocker

    public override async Task<IAsyncLocked?> TryAcquireAsync(String name, TimeSpan wait, CancellationToken cancellationToken)
    {
        var options = _getOptions?.Invoke();
        var expiryMilliseconds = options?.Expiry;
        var expiry = expiryMilliseconds.HasValue ? TimeSpan.FromMilliseconds(expiryMilliseconds.Value) : ExpiryDefault;

#if DEBUG
        if (Debugger.IsAttached) expiry = ExpiryDebug;
#endif

        var redlock = await _factory.CreateLockAsync(name, expiry).ConfigureAwait(false);
        if (redlock.IsAcquired) return new Locked(redlock);

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

                redlock = await _factory.CreateLockAsync(name, expiry).ConfigureAwait(false);
                if (redlock.IsAcquired) return new Locked(redlock);

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
        var expiryMilliseconds = options?.Expiry;
        var expiry = expiryMilliseconds.HasValue ? TimeSpan.FromMilliseconds(expiryMilliseconds.Value) : ExpiryDefault;

#if DEBUG
        if (Debugger.IsAttached) expiry = ExpiryDebug;
#endif

        var redlock = _factory.CreateLock(name, expiry);
        if (redlock.IsAcquired) return new Locked(redlock);

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

                redlock = _factory.CreateLock(name, expiry);
                if (redlock.IsAcquired) return new Locked(redlock);

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