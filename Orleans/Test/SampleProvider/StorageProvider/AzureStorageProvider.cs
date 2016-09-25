
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

        public string ConnectionString { get; set; }

        public AzureStorageProvider(IRuntime runtime, ILoggerFactory loggerFactory, ILifecycleObservable lifecycle)
        {
            logger = loggerFactory.CreateLogger<AzureStorageProvider>();
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
            logger.LogInformation("Initialize. ConnectionString: {0}", ConnectionString);
            return Task.FromResult(true);
        }
    }
}
