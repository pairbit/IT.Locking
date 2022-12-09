# IT.Locking.Medallion
[![NuGet version (IT.Locking.Medallion)](https://img.shields.io/nuget/v/IT.Locking.Medallion.svg)](https://www.nuget.org/packages/IT.Locking.Medallion)
[![NuGet pre version (IT.Locking.Medallion)](https://img.shields.io/nuget/vpre/IT.Locking.Medallion.svg)](https://www.nuget.org/packages/IT.Locking.Medallion)

Implementation of locking via DistributedLock from Medallion

[![NuGet version (DistributedLock.Core)](https://img.shields.io/nuget/v/DistributedLock.Core.svg)](https://www.nuget.org/packages/DistributedLock.Core)

## DistributedLock.FileSystem

[![NuGet version (DistributedLock.FileSystem)](https://img.shields.io/nuget/v/DistributedLock.FileSystem.svg)](https://www.nuget.org/packages/DistributedLock.FileSystem)

```csharp
    private IT.Locking.ILocker GetFileSystemLocker()
    {
        var dir = new DirectoryInfo("/var/locking");

        var provider = new global::Medallion.Threading.FileSystem.FileDistributedSynchronizationProvider(dir);

        return new IT.Locking.Medallion.Locker(provider);
    }
```

## DistributedLock.Redis

[![NuGet version (DistributedLock.Redis)](https://img.shields.io/nuget/v/DistributedLock.Redis.svg)](https://www.nuget.org/packages/DistributedLock.Redis)

```csharp
    private IT.Locking.ILocker GetRedisLocker()
    {
        static StackExchange.Redis.IDatabase GetDatabase(Int32 number)
        {
            var multiplexer = StackExchange.Redis.ConnectionMultiplexer.Connect($"localhost:6379,defaultDatabase={number},syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");
            return multiplexer.GetDatabase();
        }

        var connections = new StackExchange.Redis.IDatabase[3]
        {
            GetDatabase(0),
            GetDatabase(1),
            GetDatabase(2)
        };

        var provider = new global::Medallion.Threading.Redis.RedisDistributedSynchronizationProvider(connections);

        return new IT.Locking.Medallion.Locker(provider);
    }
```