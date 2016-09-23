
using System;

namespace Microsoft.Orleans.Lifecycle.Abstractions
{
    public interface ILifecycleObservable
    {
        IDisposable Subscribe(ILifecycleObserver observer);

        void RequestTermination(bool graceful = true);
    }
}
