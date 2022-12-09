# IT.Locking.Redis.RedLock
[![NuGet version (IT.Locking.Redis.RedLock)](https://img.shields.io/nuget/v/IT.Locking.Redis.RedLock.svg)](https://www.nuget.org/packages/IT.Locking.Redis.RedLock)
[![NuGet pre version (IT.Locking.Redis.RedLock)](https://img.shields.io/nuget/vpre/IT.Locking.Redis.RedLock.svg)](https://www.nuget.org/packages/IT.Locking.Redis.RedLock)

Implementation of locking via RedLock.net

## RedLock.net

```csharp
    private static IT.Locking.ILocker GetRedisRedLockLocker()
    {
        RedLockNet.SERedis.Configuration.RedLockMultiplexer GetDatabase(Int32 number)
        {
            var multiplexer = StackExchange.Redis.ConnectionMultiplexer.Connect($"localhost:6379,defaultDatabase={number},syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");
            var db = multiplexer.GetDatabase();
            return new RedLockNet.SERedis.Configuration.RedLockMultiplexer(multiplexer) { RedisDatabase = db.Database };
        }

        var connections = new RedLockNet.SERedis.Configuration.RedLockMultiplexer[3]
        {
            GetDatabase(0),
            GetDatabase(1),
            GetDatabase(2)
        };

        var factory = RedLockNet.SERedis.RedLockFactory.Create(connections, new RedLockNet.SERedis.Configuration.RedLockRetryConfiguration(1), null);

        return new IT.Locking.Redis.RedLock.Locker(factory);
    }
```