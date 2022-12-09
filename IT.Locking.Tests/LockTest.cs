using System.Diagnostics;

namespace IT.Locking.Tests;

public abstract class LockTest : NoLockTest
{
    private readonly ILocker _locker;

    public LockTest(ILocker locker)
    {
        _locker = locker;
    }

    protected override void InsertData(IDictionary<Guid, byte> data, byte value)
    {
        var wait = Debugger.IsAttached ? TimeSpan.FromSeconds(1) : TimeSpan.FromMilliseconds(300);

        var name = $"InsertData-{value}";

        var @lock = _locker.NewLock(name);

        //simple
        //using var locked = @lock.TryAcquire();

        //if (locked != null)
        //{
        //    if (!data.Values.Contains(value))
        //    {
        //        Task.Delay(250).Wait();
        //        base.InsertData(data, value);
        //    }
        //}
        //else
        //{
        //    Interlocked.Increment(ref _noLock);
        //}

        //good
        var status = @lock.TryAcquireWithCheck(
            _ => data.Values.Contains(value),
            _ =>
            {
                Task.Delay(250).Wait();
                base.InsertData(data, value);
            }, wait);

        if (!status) Interlocked.Increment(ref _noLock);
    }
}