
using System.Threading.Tasks;

namespace Microsoft.Orleans.Lifecycle.Abstractions
{
    public interface ILifecycleObserver
    {
        Task OnInitialize();
        Task OnStart();
        Task OnStop();
    }
}
