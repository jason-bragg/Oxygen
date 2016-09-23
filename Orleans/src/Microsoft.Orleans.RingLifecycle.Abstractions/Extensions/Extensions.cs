
using System;
using System.Threading.Tasks;
using Microsoft.Orleans.Lifecycle.Abstractions;

namespace Microsoft.Orleans.RingLifecycle.Abstractions.Extensions
{
    public static class Extensions
    {
        private static Func<Task> NoOp => () => Task.FromResult(true);

        public static IDisposable Subscribe(this IRingLifecycleObservable observable, Func<Task> onInitialize, Func<Task> onStart, Func<Task> onStop, int ring = Constants.RingZero)
        {
            if (observable == null) throw new ArgumentNullException(nameof(observable));
            if (onInitialize == null) throw new ArgumentNullException(nameof(onInitialize));
            if (onStart == null) throw new ArgumentNullException(nameof(onStart));
            if (onStop == null) throw new ArgumentNullException(nameof(onStop));

            return observable.Subscribe(new Observer(onInitialize, onStart, onStart), ring);
        }

        public static IDisposable Subscribe(this IRingLifecycleObservable observable, Func<Task> onStart, Func<Task> onStop, int ring = Constants.RingZero)
        {
            return observable.Subscribe(new Observer(NoOp, onStart, onStop), ring);
        }

        public static IDisposable Subscribe(this IRingLifecycleObservable observable, Func<Task> onInitialize, int ring = Constants.RingZero)
        {
            return observable.Subscribe(new Observer(onInitialize, NoOp, NoOp), ring);
        }

        private class Observer : ILifecycleObserver
        {
            private readonly Func<Task> onInitialize;
            private readonly Func<Task> onStart;
            private readonly Func<Task> onStop;

            public Observer(Func<Task> onInitialize, Func<Task> onStart, Func<Task> onStop)
            {
                this.onInitialize = onInitialize;
                this.onStart = onStart;
                this.onStop = onStop;
            }

            public Task OnInitialize() => onInitialize();
            public Task OnStart() => onStart();
            public Task OnStop() => onStop();
        }
    }
}
