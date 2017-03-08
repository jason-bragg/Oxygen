
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

            return observable.Subscribe(new Observer(onInitialize, onStart, onStop));
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

        public static IDisposable OnStart<TKey>(this IStagedLifecycle<TKey> lifecycle, TKey key, Func<Task> action)
        {
            return lifecycle.Start[key].OnStage(action);
        }

        public static IDisposable OnStage(this IStage stage, Func<Task> action)
        {
            TaskCompletionSource<bool> canceled = new TaskCompletionSource<bool>();
            stage.WaitOn(WrapAction(stage, action, canceled.Task));
            return new DisposableAction(() => canceled.TrySetResult(true));
        }

        public static void OnStage(this IStage stage, Func<Task> action, Task canceled)
        {
            stage.WaitOn(WrapAction(stage, action, canceled));
        }

        public static IDisposable Subscribe<TKey>(this IStagedLifecycle<TKey> lifecycle, TKey key, ILifecycleObserver observer)
        {
            TaskCompletionSource<bool> canceled = new TaskCompletionSource<bool>();
            lifecycle.Start[key].OnStage(observer.OnStart, canceled.Task);
            lifecycle.Stop[key].OnStage(observer.OnStop, canceled.Task);
            return new DisposableAction(() => canceled.TrySetResult(true));

        }

        private static async Task WrapAction(IStage stage, Func<Task> action, Task canceled)
        {
            Task completed = await Task.WhenAny(stage.Signal, canceled);
            if(completed == stage.Signal)
            {
                await action();
            }
        }

        private class DisposableAction : IDisposable
        {
            private readonly Action action;

            public DisposableAction(Action action)
            {
                this.action = action;
            }

            public void Dispose()
            {
                this.action();
            }
        }
    }
}
