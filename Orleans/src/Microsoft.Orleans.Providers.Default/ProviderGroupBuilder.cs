
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Orleans.Factory.Abstractions;
using Microsoft.Orleans.Factory.Abstractions.Extentions;
using Microsoft.Orleans.Factory.Default;
using Microsoft.Orleans.Providers.Abstractions;

namespace Microsoft.Orleans.Providers.Default
{
    public class ProviderGroupBuilder<TKey, TProvider>
        where TKey : IComparable<TKey>
        where TProvider : class
    {
        private readonly IServiceCollection servicesCollection;
        private readonly List<ConfiguredProvider> configuredProviders;

        public ProviderGroupBuilder(IServiceCollection services)
        {
            if(services == null) throw new ArgumentNullException(nameof(services));
            servicesCollection = services;
            configuredProviders = new List<ConfiguredProvider>();
        }

        public void Add<TSpecialization>(TKey key, Action<TSpecialization> configure = null)
            where TSpecialization : TProvider
        {
            if(servicesCollection.All(descriptor => descriptor.ServiceType != typeof(TSpecialization)))
            {
                servicesCollection.AddTransient(typeof(TSpecialization));
            }
            configuredProviders.Add(new SpecializedConfiguredProvider<TSpecialization>(key, configure));
        }

        public IServiceCollection Build()
        {
            servicesCollection.AddTransient<ProviderGroup>();
            servicesCollection.AddSingleton(Create);
            return servicesCollection;
        }

        private IFactory<TKey, TProvider> CreateProviderFactory(IServiceProvider serviceProvider)
        {
            IFactoryBuilder<TKey, TProvider> builder = new FactoryBuilder<TKey, TProvider>();
            foreach (ConfiguredProvider configuredProvider in configuredProviders)
            {
                builder.Add<TKey, TProvider>(configuredProvider.Key, configuredProvider.InstanceType, serviceProvider, configuredProvider.Configure);
            }
            return builder.Build();
        }

        private IProviderGroup<TKey, TProvider> Create(IServiceProvider serviceProvider)
        {
            var providerGroup = serviceProvider.GetRequiredService<ProviderGroup>();
            providerGroup.Initialize(CreateProviderFactory(serviceProvider));
            return providerGroup;
        }

        private abstract class ConfiguredProvider
        {
            protected ConfiguredProvider(TKey key)
            {
                Key = key;
            }

            public TKey Key { get; }
            public abstract Type InstanceType { get; }
            public abstract void Configure(TProvider provider);
        }

        private class SpecializedConfiguredProvider<TSpecialization> : ConfiguredProvider
            where TSpecialization : TProvider
        {
            private readonly Action<TSpecialization> configureProvider;

            public SpecializedConfiguredProvider(TKey key, Action<TSpecialization> configure = null)
                : base(key)
            {
                configureProvider = configure;
            }

            public override Type InstanceType => typeof(TSpecialization);

            public override void Configure(TProvider provider)
            {
                configureProvider?.Invoke((TSpecialization)provider);
            }

        }


        private class ProviderGroup : IProviderGroup<TKey, TProvider>
        {
            private readonly ILogger logger;

            public IReadOnlyCollection<KeyValuePair<TKey, TProvider>> Providers { get; private set; }

            public ProviderGroup(ILoggerFactory loggerFactory)
            {
                if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

                logger = loggerFactory.CreateLogger<ProviderGroup>();
            }

            public void Initialize(IFactory<TKey, TProvider> factory)
            {
                if (factory == null) throw new ArgumentNullException(nameof(factory));
                logger.LogInformation("Initialize");

                Providers = factory.Keys.ToDictionary(key => key, factory.Create);
            }
        }
    }
}
