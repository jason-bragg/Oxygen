using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Orleans.Providers.Abstractions.Extensions;
using Microsoft.Orleans.Silo.Abstractions;
using Microsoft.Orleans.StorageProvider.Abstractions;
using SampleProvider.StorageProvider;

namespace SampleProvider.Silo
{
    class SomePersistentGrain
    {
        private readonly ILogger logger;
        private readonly IStorageProvider storageProvider;

        public string ConnectionString { get; set; }

        public SomePersistentGrain(ISiloRuntime runtime, ILoggerFactory loggerFactory, IStorageProviderGroup storageProviderGroup)
        {
            logger = loggerFactory.CreateLogger<AzureStorageProvider>();
            // assuming grain know which provider it want's via attribute, or something.
            storageProvider = storageProviderGroup.GetProvider("Azure");
        }

        public async Task OnActivateAsync()
        {
            logger.LogInformation("OnActivateAsync");
            await storageProvider.ReadAsync();
        }
        public async Task OnDeactivateAsync()
        {
            logger.LogInformation("OnActivateAsync");
            await storageProvider.WriteAsync();
        }
    }
}
