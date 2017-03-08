
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Orleans.Lifecycle.Abstractions;
using Microsoft.Orleans.Lifecycle.Abstractions.Extensions;


namespace Microsoft.Orleans.Lifecycle.Default
{
    public class StagedLifecycle<TStageKey> : ILifecycleObserver, ILifecycleObservable<TStageKey>
    {
        private readonly Staged staged;
        TStageKey highKey;

        public StagedLifecycle(ILifecycleObservable lifecycle)
        {
            lifecycle.Subscribe(this);
            staged = new Staged();
            highKey = default(TStageKey);
        }

        public async Task OnStart()
        {
            Exception startException = null;
            foreach (var stageKey in staged.StartGroup.Map.Keys.OrderBy(k => k))
            {
                highKey = stageKey;
                Staged.StageGroup.Stage stage = staged.StartGroup.Map[stageKey];
                if(startException != null)
                {
                    stage.SignalCompletion.SetResult(true);
                }
                else
                {
                    stage.SignalCompletion.SetCanceled();
                }
                try
                {
                    await Task.WhenAll(stage);
                } catch(Exception ex)
                {
                    startException = ex;
                }
            }
            if (startException != null)
            {
                await OnStop();
                throw new OperationCanceledException("Start canceled to faiures.", startException);
            }
        }

        public async Task OnStop()
        {
            bool skip = true;
            foreach (var stageKey in staged.StopGroup.Map.Keys.OrderByDescending(k => k))
            {
                if(skip && highKey.Equals(stageKey))
                {
                    skip = false;
                }
                if(skip)
                {
                    continue;
                }
                highKey = stageKey;
                Staged.StageGroup.Stage stage = staged.StopGroup.Map[stageKey];
                stage.SignalCompletion.SetResult(true);
                try
                {
                    await Task.WhenAll(stage);
                }
                catch
                {
                }
            }
        }

        public void RequestTermination(bool graceful = true)
        {
            /// TBD
        }

        public IDisposable Subscribe(TStageKey key, ILifecycleObserver observer)
        {
            return staged.Subscribe(key, observer);
        }

        private class Staged : IStagedLifecycle<TStageKey>
        {
            public StageGroup StartGroup { get; } = new StageGroup();

            public StageGroup StopGroup { get; } = new StageGroup();

            public IStagedGroup<TStageKey> Start => StartGroup;

            public IStagedGroup<TStageKey> Stop => StopGroup;

            public class StageGroup : IStagedGroup<TStageKey>
            {
                public ConcurrentDictionary<TStageKey, Stage> Map { get; } = new ConcurrentDictionary<TStageKey, Stage>();

                public IStage this[TStageKey key]
                {
                    get
                    {
                        return Map.GetOrAdd(key, (k) => new Stage());
                    }
                }

                public class Stage : ConcurrentBag<Task>, IStage
                {
                    public TaskCompletionSource<bool> SignalCompletion { get; } = new TaskCompletionSource<bool>();

                    public Task Signal => SignalCompletion.Task;

                    public void WaitOn(Task completion)
                    {
                        Add(completion);
                    }
                }
            }
        }
    }
}
