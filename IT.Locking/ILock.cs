using System;
using System.Threading;

namespace IT.Locking;

public interface ILock : IAsyncLock
{
    ILocked? TryAcquire(TimeSpan wait = default, CancellationToken cancellationToken = default);

    T? TryAcquireWithCheck<T>(Func<CancellationToken, T?> check, Func<CancellationToken, T> getResult,
        TimeSpan wait = default, CancellationToken cancellationToken = default);

    Boolean TryAcquireWithCheck(Func<CancellationToken, Boolean> check, Action<CancellationToken> action,
        TimeSpan wait = default, CancellationToken cancellationToken = default);
}