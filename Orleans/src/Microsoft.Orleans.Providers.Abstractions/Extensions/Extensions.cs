
using System;
using System.Linq;

namespace Microsoft.Orleans.Providers.Abstractions.Extensions
{
    public static class Extensions
    {
        public static bool TryGetProvider<TKey,TProvider>(this IProviderGroup<TKey, TProvider> providerGroup, TKey key, out TProvider provider)
            where TKey : IComparable<TKey>
            where TProvider : class
        {
            provider = providerGroup.Providers.FirstOrDefault(kvp => kvp.Key.Equals(key)).Value;
            return provider != default(TProvider);
        }

        public static TProvider GetProvider<TKey, TProvider>(this IProviderGroup<TKey, TProvider> providerGroup, TKey key)
            where TKey : IComparable<TKey>
            where TProvider : class
        {
            TProvider provider;
            return providerGroup.TryGetProvider(key, out provider)
                ? provider
                : default(TProvider);
        }
    }
}
