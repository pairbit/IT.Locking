using Microsoft.Extensions.Logging.Abstractions;
using StackExchange.Redis;

namespace IT.Locking.Tests;

public class RedisLockTest : LockTest
{
    public RedisLockTest() : base(GetLocker())
    {

    }

    private static ILocker GetLocker()
    {
        var multiplexer = ConnectionMultiplexer.Connect("localhost:6379,defaultDatabase=0,syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");
        var db = multiplexer.GetDatabase();
        var logger = NullLogger<Redis.Locker>.Instance;

        return new Redis.Locker(db);
    }
}