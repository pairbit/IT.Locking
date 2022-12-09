using Medallion.Threading;
using System.Threading.Tasks;

namespace IT.Locking.Medallion;

internal class Locked : ILocked
{
    private readonly IDistributedSynchronizationHandle _handle;

    public Locked(IDistributedSynchronizationHandle handle)
    {
        _handle = handle;
    }

    public void Dispose() => _handle.Dispose();

    public ValueTask DisposeAsync() => _handle.DisposeAsync();
}