# IT.Locking.Redis
[![NuGet version (IT.Locking.Redis)](https://img.shields.io/nuget/v/IT.Locking.Redis.svg)](https://www.nuget.org/packages/IT.Locking.Redis)
[![NuGet pre version (IT.Locking.Redis)](https://img.shields.io/nuget/vpre/IT.Locking.Redis.svg)](https://www.nuget.org/packages/IT.Locking.Redis)

Implementation of locking via StackExchange.Redis (simple)

## StackExchange.Redis

```csharp
    private IT.Locking.ILocker GetRedisLocker()
    {
        var multiplexer = StackExchange.Redis.ConnectionMultiplexer.Connect("localhost:6379,defaultDatabase=0,syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");
        var db = multiplexer.GetDatabase();

        return new IT.Locking.Redis.Locker(db);
    }
```