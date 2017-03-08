
using System.Threading.Tasks;

namespace Microsoft.Orleans.Lifecycle.Abstractions
{
    public interface ISharedState<T>
    {
        T State { get; }
        Task<ISharedState<T>> NextAsync { get; }
    }

    public interface IStage
    {
        Task Signal { get; }
        void WaitOn(Task completion);
    }

    public interface IWaiter
    {
        void WaitOn(Task completion);
    }

    public interface IStagedGroup<in TStageKey>
    {
        IStage this[TStageKey key] { get; }
    }

    public interface IStagedLifecycle<in TStageKey>
    {
        IStagedGroup<TStageKey> Start { get; }
        IStagedGroup<TStageKey> Stop { get; }
    }
}
