
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Orleans.Providers.Default.Extensions
{
    public static class Extensions
    {
        public static ProviderGroupServiceBuilder<TKey, TProvider> AddProviderGroup<TKey,TProvider>(this IServiceCollection serviceCollection)
            where TKey : IComparable<TKey>
            where TProvider : class
        {
            return new ProviderGroupServiceBuilder<TKey, TProvider>(serviceCollection);
        }
    }
}
