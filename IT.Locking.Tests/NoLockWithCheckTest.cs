namespace IT.Locking.Tests;

public class NoLockWithCheckTest : NoLockTest
{
    protected override void InsertData(IDictionary<Guid, byte> data, byte value)
    {
        if (!data.Values.Contains(value))
        {
            Task.Delay(10).Wait();
            base.InsertData(data, value);
        }
    }
}