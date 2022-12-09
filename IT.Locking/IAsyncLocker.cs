using System;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Locking;

public interface IAsyncLocker
{
    IAsyncLock NewAsyncLock(String name);

    Task<IAsyncLocked?> TryAcquireAsync(String name, TimeSpan wait = default, CancellationToken cancellationToken = default);

    Task<T?> TryAcquireWithCheckAsync<T>(String name,
        Func<CancellationToken, Task<T?>> checkAsync, Func<CancellationToken, Task<T>> getResultAsync,
        TimeSpan wait = default, CancellationToken cancellationToken = default);

    Task<Boolean> TryAcquireWithCheckAsync(String name,
        Func<CancellationToken, Task<Boolean>> checkAsync, Func<CancellationToken, Task> doAsync,
        TimeSpan wait = default, CancellationToken cancellationToken = default);
}