using System;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Locking;

public static class xIAsyncLock
{
    /// <exception cref="TimeoutException"></exception>
    public static async Task<IAsyncLocked> AcquireAsync(this IAsyncLock @lock, TimeSpan wait = default, CancellationToken cancellationToken = default)
        => await @lock.TryAcquireAsync(wait, cancellationToken).ConfigureAwait(false) ?? throw new TimeoutException($"Lock '{@lock.Name}'");

    /// <exception cref="TimeoutException"></exception>
    public static async Task<T> AcquireWithCheckAsync<T>(this IAsyncLock @lock,
        Func<CancellationToken, Task<T?>> checkAsync, Func<CancellationToken, Task<T>> getResultAsync,
        TimeSpan wait = default, CancellationToken cancellationToken = default)
        => await @lock.TryAcquireWithCheckAsync(checkAsync, getResultAsync, wait, cancellationToken).ConfigureAwait(false) ?? throw new TimeoutException($"Lock '{@lock.Name}'");

    /// <exception cref="TimeoutException"></exception>
    public static async Task AcquireWithCheckAsync(this IAsyncLock @lock,
        Func<CancellationToken, Task<Boolean>> checkAsync, Func<CancellationToken, Task> doAsync,
        TimeSpan wait = default, CancellationToken cancellationToken = default)
    {
        if (!await @lock.TryAcquireWithCheckAsync(checkAsync, doAsync, wait, cancellationToken).ConfigureAwait(false))
            throw new TimeoutException($"Lock '{@lock.Name}'");
    }
}