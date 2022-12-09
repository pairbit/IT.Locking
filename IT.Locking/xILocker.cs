using System;
using System.Threading;

namespace IT.Locking;

public static class xILocker
{
    /// <exception cref="TimeoutException"></exception>
    public static ILocked Acquire(this ILocker locker, String name, TimeSpan wait = default, CancellationToken cancellationToken = default)
        => locker.TryAcquire(name, wait, cancellationToken) ?? throw new TimeoutException($"Lock '{name}'");

    /// <exception cref="TimeoutException"></exception>
    public static T AcquireWithCheck<T>(this ILocker locker, String name, Func<CancellationToken, T?> check, Func<CancellationToken, T> getResult,
        TimeSpan wait = default, CancellationToken cancellationToken = default)
        => locker.TryAcquireWithCheck(name, check, getResult, wait, cancellationToken) ?? throw new TimeoutException($"Lock '{name}'");
    
    /// <exception cref="TimeoutException"></exception>
    public static void AcquireWithCheck(this ILocker locker, String name, Func<CancellationToken, Boolean> check, Action<CancellationToken> action,
        TimeSpan wait = default, CancellationToken cancellationToken = default)
    {
        if (!locker.TryAcquireWithCheck(name, check, action, wait, cancellationToken))
            throw new TimeoutException($"Lock '{name}'");
    }
}