#if NETSTANDARD2_0 || NETSTANDARD2_1

using System.Threading;

namespace System;

internal static class _Random
{
    private static readonly Random _global = new();
    private static readonly ThreadLocal<Random> _local = new(() =>
    {
        int seed;
        lock (_global)
        {
            seed = _global.Next();
        }
        return new Random(seed);
    });

    //https://devblogs.microsoft.com/pfxteam/getting-random-numbers-in-a-thread-safe-way/
    public static Random Shared => _local.Value;
}

#endif