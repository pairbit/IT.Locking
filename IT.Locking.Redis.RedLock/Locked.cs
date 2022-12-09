using RedLockNet;
using System.Threading.Tasks;

namespace IT.Locking.Redis.RedLock;

internal class Locked : ILocked
{
    private readonly IRedLock _redlock;

    public Locked(IRedLock redlock)
    {
        _redlock = redlock;
    }

    public void Dispose() => _redlock.Dispose();

    public ValueTask DisposeAsync() => _redlock.DisposeAsync();
}