# IT.Locking
[![NuGet version (IT.Locking)](https://img.shields.io/nuget/v/IT.Locking.svg)](https://www.nuget.org/packages/IT.Locking)
[![NuGet pre version (IT.Locking)](https://img.shields.io/nuget/vpre/IT.Locking.svg)](https://www.nuget.org/packages/IT.Locking)

Interfaces of locking

## Acquire

```csharp
    private void Locker(ILocker locker)
    {
        using var locked = locker.Acquire("LockName", TimeSpan.FromSeconds(1));

        if (locked != null)
        {
            // do
        }

        Lock(locker.NewLock("LockName2"));
    }

    private async Task LockerAsync(IAsyncLocker locker)
    {
        await using var locked = await locker.AcquireAsync("LockName", TimeSpan.FromSeconds(1)).ConfigureAwait(false);

        if (locked != null)
        {
            // do
        }

        await LockAsync(locker.NewAsyncLock("LockName2")).ConfigureAwait(false);
    }

    private void Lock(ILock @lock)
    {
        using var locked = @lock.Acquire(TimeSpan.FromSeconds(1));

        if (locked != null)
        {
            // do
        }
    }

    private async Task LockAsync(IAsyncLock @lock)
    {
        await using var locked = await @lock.AcquireAsync(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

        if (locked != null)
        {
            // do
        }
    }
```