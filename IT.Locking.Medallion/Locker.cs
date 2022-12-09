using Medallion.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Locking.Medallion;

public class Locker : Locking.Locker
{
    private readonly IDistributedLockProvider _provider;
    private readonly Int32? _retryMin;

    protected override Int32? RetryMin => _retryMin;

    public Locker(IDistributedLockProvider provider, Int32? retryMin = null)
    {
        _provider = provider;
        _retryMin = retryMin;
    }

    public override ILocked? TryAcquire(String name, TimeSpan wait, CancellationToken cancellationToken)
    {
        var handle = _provider.CreateLock(name).TryAcquire(wait, cancellationToken);
        return handle is not null ? new Locked(handle) : null;
    }

    public override async Task<IAsyncLocked?> TryAcquireAsync(String name, TimeSpan wait, CancellationToken cancellationToken)
    {
        var handle = await _provider.CreateLock(name).TryAcquireAsync(wait, cancellationToken).ConfigureAwait(false);
        return handle is not null ? new Locked(handle) : null;
    }
}