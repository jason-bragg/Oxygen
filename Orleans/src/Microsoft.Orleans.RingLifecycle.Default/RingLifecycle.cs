
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Orleans.Lifecycle.Abstractions;
using Microsoft.Orleans.RingLifecycle.Abstractions;

namespace Microsoft.Orleans.RingLifecycle.Default
{
    public class Lifecycle : ILifecycleObserver, IRingLifecycleObservable
    {
        private readonly ConcurrentDictionary<object, OrderedObserver> subscribers;

        public Lifecycle(ILifecycleObservable lifecycle)
        {
            lifecycle.Subscribe(this);
            subscribers = new ConcurrentDictionary<object, OrderedObserver>();
        }

        public Task OnStart()
        {
            return Task.WhenAll(subscribers.OrderBy(kvp => kvp.Value.Ring).Select(kvp => kvp.Value.Observer.OnStart()));
        }

        public Task OnStop()
        {
            // stop in reverse startup order
            return Task.WhenAll(subscribers.OrderByDescending(kvp => kvp.Value.Ring).Select(kvp => kvp.Value.Observer.OnStop()));
        }

        public void RequestTermination(bool graceful = true)
        {
            /// TBD
        }

        public IDisposable Subscribe(ILifecycleObserver observer, int ring = Constants.RingZero)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            var key = new object();
            subscribers.TryAdd(key, new OrderedObserver(observer, ring));
            return new Disposable(() => Remove(key));
        }

        private void Remove(object key)
        {
            OrderedObserver o;
            subscribers.TryRemove(key, out o);
        }

        private class Disposable : IDisposable
        {
            private readonly Action dispose;

            public Disposable(Action dispose)
            {
                this.dispose = dispose;
            }

            public void Dispose()
            {
                this.dispose();
            }
        }

        private class OrderedObserver
        {
            public ILifecycleObserver Observer { get; }
            public int Ring { get; }

            public OrderedObserver(ILifecycleObserver observer, int ring)
            {
                Observer = observer;
                Ring = ring;
            }
        }
    }
}
