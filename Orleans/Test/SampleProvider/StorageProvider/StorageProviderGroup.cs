
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Orleans.Factory.Abstractions;
using Microsoft.Orleans.Lifecycle.Abstractions;
using Microsoft.Orleans.StorageProvider.Abstractions;
using Microsoft.Orleans.Lifecycle.Abstractions.Extensions;

namespace SampleProvider.StorageProvider
{
    public class StorageProviderGroup : IStorageProviderGroup
    {
        private readonly ILogger logger;
        private readonly IFactory<string, IStorageProvider> factory;

        public IReadOnlyCollection<string> ConfiguredProviders { get; set; }
        public IReadOnlyCollection<KeyValuePair<string, IStorageProvider>> Providers { get; private set; }
        
        public StorageProviderGroup(ILifecycleObservable lifecycle, ILoggerFactory loggerFactory, IFactory<string, IStorageProvider> providerFactory )
        {
            if (lifecycle == null) throw new ArgumentNullException(nameof(lifecycle));
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            if (providerFactory == null) throw new ArgumentNullException(nameof(providerFactory));

            logger = loggerFactory.CreateLogger<StorageProviderGroup>();
            lifecycle.Subscribe(Initialize);
            factory = providerFactory;
        }

        private Task Initialize()
        {
            logger.LogInformation("Initialize");

            Providers = ConfiguredProviders.ToDictionary(name => name, name => factory.Create(name));
            return Task.FromResult(true);
        }
    }
}
