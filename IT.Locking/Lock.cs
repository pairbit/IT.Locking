using System;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Locking;

internal class Lock : ILock
{
    private readonly ILocker _locker;
    private readonly String _name;

    public String Name => _name;

    public Lock(String name, ILocker locker)
    {
        _locker = locker;
        _name = name;
    }

    public ILocked? TryAcquire(TimeSpan wait, CancellationToken cancellationToken)
        => _locker.TryAcquire(_name, wait, cancellationToken);

    public T? TryAcquireWithCheck<T>(Func<CancellationToken, T?> check, Func<CancellationToken, T> getResult,
        TimeSpan wait, CancellationToken cancellationToken)
        => _locker.TryAcquireWithCheck(_name, check, getResult, wait, cancellationToken);

    public Boolean TryAcquireWithCheck(Func<CancellationToken, Boolean> check, Action<CancellationToken> action,
        TimeSpan wait, CancellationToken cancellationToken)
        => _locker.TryAcquireWithCheck(_name, check, action, wait, cancellationToken);

    public Task<IAsyncLocked?> TryAcquireAsync(TimeSpan wait, CancellationToken cancellationToken)
        => _locker.TryAcquireAsync(_name, wait, cancellationToken);

    public Task<T?> TryAcquireWithCheckAsync<T>(
        Func<CancellationToken, Task<T?>> checkAsync, Func<CancellationToken, Task<T>> getResultAsync,
        TimeSpan wait, CancellationToken cancellationToken)
        => _locker.TryAcquireWithCheckAsync(_name, checkAsync, getResultAsync, wait, cancellationToken);

    public Task<Boolean> TryAcquireWithCheckAsync(
        Func<CancellationToken, Task<Boolean>> checkAsync, Func<CancellationToken, Task> doAsync,
        TimeSpan wait, CancellationToken cancellationToken)
        => _locker.TryAcquireWithCheckAsync(_name, checkAsync, doAsync, wait, cancellationToken);
}