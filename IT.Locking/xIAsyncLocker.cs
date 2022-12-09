using System;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Locking;

public static class xIAsyncLocker
{
    /// <exception cref="TimeoutException"></exception>
    public static async Task<IAsyncLocked> AcquireAsync(this IAsyncLocker locker, String name, TimeSpan wait = default, CancellationToken cancellationToken = default)
        => await locker.TryAcquireAsync(name, wait, cancellationToken).ConfigureAwait(false) ?? throw new TimeoutException($"Lock '{name}'");

    /// <exception cref="TimeoutException"></exception>
    public static async Task<T> AcquireWithCheckAsync<T>(this IAsyncLocker locker, String name,
        Func<CancellationToken, Task<T?>> checkAsync, Func<CancellationToken, Task<T>> getResultAsync,
        TimeSpan wait = default, CancellationToken cancellationToken = default)
        => await locker.TryAcquireWithCheckAsync(name, checkAsync, getResultAsync, wait, cancellationToken).ConfigureAwait(false) ?? throw new TimeoutException($"Lock '{name}'");

    /// <exception cref="TimeoutException"></exception>
    public static async Task AcquireWithCheckAsync(this IAsyncLocker locker, String name,
        Func<CancellationToken, Task<Boolean>> checkAsync, Func<CancellationToken, Task> doAsync,
        TimeSpan wait = default, CancellationToken cancellationToken = default)
    {
        if (!await locker.TryAcquireWithCheckAsync(name, checkAsync, doAsync, wait, cancellationToken).ConfigureAwait(false))
            throw new TimeoutException($"Lock '{name}'");
    }
}