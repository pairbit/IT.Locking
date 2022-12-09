using Microsoft.Extensions.Logging.Abstractions;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace IT.Locking.Tests;

public class RedisRedLockTest : LockTest
{
    public RedisRedLockTest() : base(GetLocker())
    {

    }

    private static ILocker GetLocker()
    {
        RedLockMultiplexer Create(Int32 number)
        {
            var multiplexer = ConnectionMultiplexer.Connect($"localhost:6379,defaultDatabase={number},syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");
            var db = multiplexer.GetDatabase();
            return new RedLockMultiplexer(multiplexer) { RedisDatabase = db.Database };
        }

        var redLockMultiplexers = new List<RedLockMultiplexer>(3);

        redLockMultiplexers.Add(Create(0));

        redLockMultiplexers.Add(Create(1));

        redLockMultiplexers.Add(Create(2));

        var factory = RedLockFactory.Create(redLockMultiplexers, new RedLockRetryConfiguration(1), null);

        var logger = NullLogger<Redis.RedLock.Locker>.Instance;

        return new Redis.RedLock.Locker(factory);
    }
}