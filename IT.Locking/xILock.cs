using System;
using System.Threading;

namespace IT.Locking;

public static class xILock
{
    /// <exception cref="TimeoutException"></exception>
    public static ILocked Acquire(this ILock @lock, TimeSpan wait = default, CancellationToken cancellationToken = default)
        => @lock.TryAcquire(wait, cancellationToken) ?? throw new TimeoutException($"Lock '{@lock.Name}'");

    /// <exception cref="TimeoutException"></exception>
    public static T AcquireWithCheck<T>(this ILock @lock, Func<CancellationToken, T?> check, Func<CancellationToken, T> getResult,
        TimeSpan wait = default, CancellationToken cancellationToken = default)
        => @lock.TryAcquireWithCheck(check, getResult, wait, cancellationToken) ?? throw new TimeoutException($"Lock '{@lock.Name}'");
    
    /// <exception cref="TimeoutException"></exception>
    public static void AcquireWithCheck(this ILock @lock, Func<CancellationToken, Boolean> check, Action<CancellationToken> action,
        TimeSpan wait = default, CancellationToken cancellationToken = default)
    {
        if (!@lock.TryAcquireWithCheck(check, action, wait, cancellationToken))
            throw new TimeoutException($"Lock '{@lock.Name}'");
    }
}