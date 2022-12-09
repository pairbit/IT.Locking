using System;

namespace IT.Locking;

public interface ILocked : IAsyncLocked, IDisposable
{

}