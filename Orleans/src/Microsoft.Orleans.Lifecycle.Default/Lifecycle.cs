
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Orleans.Lifecycle.Abstractions;

namespace Microsoft.Orleans.Lifecycle.Default
{
    public class Lifecycle : ILifecycleObserver, ILifecycleObservable
    {
        private readonly ConcurrentDictionary<object, ILifecycleObserver> subscribers;

        public Lifecycle()
        {
            subscribers = new ConcurrentDictionary<object, ILifecycleObserver>();
        }

        public Task OnInitialize()
        {
            return Task.WhenAll(subscribers.Select(kvp => kvp.Value.OnInitialize()));
        }

        public Task OnStart()
        {
            return Task.WhenAll(subscribers.Select(kvp => kvp.Value.OnStart()));
        }

        public Task OnStop()
        {
            return Task.WhenAll(subscribers.Select(kvp => kvp.Value.OnStop()));
        }

        public void RequestTermination(bool graceful = true)
        {
            /// TBD
        }

        public IDisposable Subscribe(ILifecycleObserver observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            var key = new object();
            subscribers.TryAdd(key, observer);
            return new Disposable(() => Remove(key));
        }

        private void Remove(object key)
        {
            ILifecycleObserver o;
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
                dispose();
            }
        }
    }
}
