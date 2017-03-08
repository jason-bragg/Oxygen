
using System;

namespace Microsoft.Orleans.Lifecycle.Abstractions
{
    public interface ILifecycleObservable
    {
        IDisposable Subscribe(ILifecycleObserver observer);

        void RequestTermination(bool graceful = true);
    }

    public interface ILifecycleObservable<TKey>
    {
        IDisposable Subscribe(TKey key, ILifecycleObserver observer);

        void RequestTermination(bool graceful = true);
    }
}
