
using System.Threading.Tasks;

namespace Microsoft.Orleans.StorageProvider.Abstractions
{
    public interface IStorageProvider
    {
        Task ReadAsync();
        Task WriteAsync();
        Task ClearAsync();
    }
}
