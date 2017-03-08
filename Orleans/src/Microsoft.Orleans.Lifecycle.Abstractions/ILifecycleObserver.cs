
using System.Threading.Tasks;

namespace Microsoft.Orleans.Lifecycle.Abstractions
{
    public interface ILifecycleObserver
    {
        Task OnStart();
        Task OnStop();
    }
}
