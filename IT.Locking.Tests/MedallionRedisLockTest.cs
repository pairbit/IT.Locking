using StackExchange.Redis;

namespace IT.Locking.Tests;

public class MedallionRedisLockTest : LockTest
{
    public MedallionRedisLockTest() : base(GetLocker())
    {

    }

    private static ILocker GetLocker()
    {
        IDatabase Create(Int32 number)
        {
            var multiplexer = ConnectionMultiplexer.Connect($"localhost:6379,defaultDatabase={number},syncTimeout=5000,allowAdmin=False,connectTimeout=5000,ssl=False,abortConnect=False");
            return multiplexer.GetDatabase();
        }

        var connections = new List<IDatabase>(3);

        connections.Add(Create(0));

        connections.Add(Create(1));

        connections.Add(Create(2));

        var provider = new global::Medallion.Threading.Redis.RedisDistributedSynchronizationProvider(connections);

        return new Medallion.Locker(provider);
    }
}