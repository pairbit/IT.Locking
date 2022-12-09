using StackExchange.Redis;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace IT.Locking.Redis;

internal class Locked : ILocked
{
    internal static readonly String StringDeleteIfEqual;

    static Locked()
    {
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream("IT.Locking.Redis.StringDeleteIfEqual.lua");

        if (stream is null) throw new InvalidOperationException("Script 'StringDeleteIfEqual.lua' not found");

        using var reader = new StreamReader(stream);

        StringDeleteIfEqual = reader.ReadToEnd();
    }

    private readonly IDatabase _db;
    private readonly RedisKey _key;
    private readonly RedisValue _value;
    private Boolean _disposed;

    public Locked(IDatabase db, RedisKey key, RedisValue value)
    {
        _db = db;
        _key = key;
        _value = value;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            var deleted = (Boolean)_db.ScriptEvaluate(StringDeleteIfEqual, new RedisKey[] { _key }, new RedisValue[] { _value });
            //if (!deleted) throw new ArgumentException();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            _disposed = true;
            var deleted = (Boolean)await _db.ScriptEvaluateAsync(StringDeleteIfEqual, new RedisKey[] { _key }, new RedisValue[] { _value }).ConfigureAwait(false);
            //if (!deleted) throw new ArgumentException();
        }
    }
}