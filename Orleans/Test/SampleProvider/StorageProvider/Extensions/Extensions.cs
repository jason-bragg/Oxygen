
using System;
using Microsoft.Orleans.Providers.Default;
using Microsoft.Orleans.StorageProvider.Abstractions;

namespace SampleProvider.StorageProvider.Extensions
{
    public static class Extensions
    {
        public static ProviderGroupBuilder<TKey, IStorageProvider> AddAzureStorageProvider<TKey>(this ProviderGroupBuilder<TKey, IStorageProvider> builder, TKey key, string connectionString)
            where TKey : IComparable<TKey>
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            builder.Add<AzureStorageProvider>(key, (provider) => provider.ConnectionString = connectionString);
            return builder;
        }
    }
}
