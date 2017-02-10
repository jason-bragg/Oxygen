
namespace Microsoft.Orleans.Contextualizer.Abstractions
{
    public interface IContextualizedServiceProvider<TContext, out TService>
        where TService : class
    {
        TService GetService(TContext key);
    }
}
