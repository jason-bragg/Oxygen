
using System;
using System.Threading.Tasks;

namespace Microsoft.Orleans.Lifecycle.Abstractions.Extensions
{
    public static class Extensions
    {
        private static Func<Task> NoOp => () => Task.FromResult(true);

        public static IDisposable Subscribe(this ILifecycleObservable observable, Func<Task> onInitialize, Func<Task> onStart, Func<Task> onStop)
        {
            if (observable == null) throw new ArgumentNullException(nameof(observable)); 
            if (onInitialize == null) throw new ArgumentNullException(nameof(onInitialize));
            if (onStart == null) throw new ArgumentNullException(nameof(onStart));
            if (onStop == null) throw new ArgumentNullException(nameof(onStop));

            return observable.Subscribe(new Observer(onInitialize, onStart, onStart));
        }

        public static IDisposable Subscribe(this ILifecycleObservable observable, Func<Task> onStart, Func<Task> onStop)
        {
            return observable.Subscribe(new Observer(NoOp, onStart, onStop));
        }

        public static IDisposable Subscribe(this ILifecycleObservable observable, Func<Task> onInitialize)
        {
            return observable.Subscribe(new Observer(onInitialize, NoOp, NoOp));
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
