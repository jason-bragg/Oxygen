
using Microsoft.Orleans.Providers.Abstractions;

namespace Microsoft.Orleans.StorageProvider.Abstractions
{
    interface IStorageProviderGroup : IProviderGroup<string,IStorageProvider>
    {
    }
}
