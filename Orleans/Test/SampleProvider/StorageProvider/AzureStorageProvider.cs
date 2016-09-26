
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Orleans.StorageProvider.Abstractions;
using Microsoft.Orleans.Lifecycle.Abstractions;
using Microsoft.Orleans.Silo.Abstractions;
using Microsoft.Orleans.Lifecycle.Abstractions.Extensions;

namespace SampleProvider.StorageProvider
{
    public class AzureStorageProvider : IStorageProvider
    {
        private readonly ILogger logger;
        private readonly ILifecycleObservable lifecycle;
        private string connectionString;

        public AzureStorageProvider(IRuntime runtime, ILoggerFactory loggerFactory, ILifecycleObservable lifecycleObservable)
        {
            logger = loggerFactory.CreateLogger<AzureStorageProvider>();
            lifecycle = lifecycleObservable;
        }

        public  void Configure(string azureConnectionString)
        {
            connectionString = azureConnectionString;
            lifecycle.Subscribe(Initialize);
        }

        public Task ReadAsync()
        {
            logger.LogInformation("ReadAsync");
            return Task.FromResult(true);
        }

        public Task WriteAsync()
        {
            logger.LogInformation("WriteAsync");
            return Task.FromResult(true);
        }

        public Task ClearAsync()
        {
            logger.LogInformation("ClearAsync");
            return Task.FromResult(true);
        }

        private Task Initialize()
        {
            logger.LogInformation("Initialize. ConnectionString: {0}", connectionString);
            return Task.FromResult(true);
        }
    }
}
