using System;
using System.Threading;

namespace IT.Locking;

public interface ILocker : IAsyncLocker
{
    ILock NewLock(String name);

    ILocked? TryAcquire(String name, TimeSpan wait = default, CancellationToken cancellationToken = default);

    T? TryAcquireWithCheck<T>(String name, Func<CancellationToken, T?> check, Func<CancellationToken, T> getResult,
        TimeSpan wait = default, CancellationToken cancellationToken = default);

    Boolean TryAcquireWithCheck(String name, Func<CancellationToken, Boolean> check, Action<CancellationToken> action,
        TimeSpan wait = default, CancellationToken cancellationToken = default);
}