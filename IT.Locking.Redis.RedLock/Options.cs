using System;

namespace IT.Locking.Redis.RedLock;

public record Options
{
    public Int32? Expiry { get; set; }

    public Int32? RetryMin { get; set; }

    public Int32? RetryMax { get; set; }
}