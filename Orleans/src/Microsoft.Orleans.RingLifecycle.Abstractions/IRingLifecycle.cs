
using System;
using Microsoft.Orleans.Lifecycle.Abstractions;

namespace Microsoft.Orleans.RingLifecycle.Abstractions
{
    public interface IRingLifecycleObservable
    {
        IDisposable Subscribe(ILifecycleObserver observer, int ring = Constants.RingZero);

        void RequestTermination(bool graceful = true);
    }
}
