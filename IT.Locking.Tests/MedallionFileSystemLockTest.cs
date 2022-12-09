namespace IT.Locking.Tests;

public class MedallionFileSystemLockTest : LockTest
{
    public MedallionFileSystemLockTest() : base(GetLocker())
    {

    }

    private static ILocker GetLocker()
    {
        var dir = new DirectoryInfo(@"C:\var\locking");

        var provider = new global::Medallion.Threading.FileSystem.FileDistributedSynchronizationProvider(dir);

        return new Medallion.Locker(provider);
    }
}