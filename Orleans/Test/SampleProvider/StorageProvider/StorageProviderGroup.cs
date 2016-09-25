
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

        public IReadOnlyCollection<KeyValuePair<string, IStorageProvider>> Providers { get; private set; }
        
        public StorageProviderGroup(ILoggerFactory loggerFactory, IFactory<string, IStorageProvider> providerFactory )
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            if (providerFactory == null) throw new ArgumentNullException(nameof(providerFactory));

            logger = loggerFactory.CreateLogger<StorageProviderGroup>();
            factory = providerFactory;
        }

        public void Initialize(IEnumerable<string> providers)
        {
            if (providers == null) throw new ArgumentNullException(nameof(providers));
            logger.LogInformation("Initialize");

            Providers = providers.ToDictionary(name => name, name => factory.Create(name));
        }
    }
}
