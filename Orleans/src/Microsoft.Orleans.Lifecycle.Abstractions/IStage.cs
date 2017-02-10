
using System.Threading.Tasks;

namespace Microsoft.Orleans.Lifecycle.Abstractions
{
    public interface IStage
    {
        Task Signal { get; }
        void WaitOn(Task completion);
    }

    public interface IStagedLifecyle<in TStage>
    {
        Task GetSignal(TStage stage);

        Task WaitOn(TStage stage, Task completion);
    }
}
