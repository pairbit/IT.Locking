using System.Collections.Concurrent;
using System.Diagnostics;

namespace IT.Locking.Tests;

public class NoLockTest
{
    protected Int32 _noLock = 0;
    private readonly Random _random = new();
    private const Int32 Count = 500;

    [Test]
    public Task Test() => Parallel(InsertData);

    protected async Task Parallel(Action<IDictionary<Guid, Byte>> action)
    {
        var data = new ConcurrentDictionary<Guid, Byte>();
        var tasks = new Task[Debugger.IsAttached ? 1 : Environment.ProcessorCount];

        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int i = 0; i < Count; i++)
                {
                    action(data);
                }
            });
        }

        await Task.WhenAll(tasks);

        Console.WriteLine($"noLock : {_noLock}");

        Console.WriteLine($"{data.Count} from {Count * tasks.Length} (256 unique)");

        foreach (var item in data.OrderBy(x => x.Value))
        {
            Console.WriteLine($"{item.Key} - {item.Value}");
        }
    }

    private void InsertData(IDictionary<Guid, Byte> data) 
        => InsertData(data, (byte)_random.Next(0, 256));

    protected virtual void InsertData(IDictionary<Guid, Byte> data, Byte value)
    {
        data.Add(Guid.NewGuid(), value);
    }
}