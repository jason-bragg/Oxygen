using Microsoft.Extensions.DependencyInjection;
using Microsoft.Orleans.Contextualizer.Abstractions;
using Microsoft.Orleans.Factory.Abstractions;
using Microsoft.Orleans.Factory.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Orleans.Contextualizer.Default
{
    public class ContextualizedServiceProviderGuilder<TContext, TService>
        where TContext : IComparable<TContext>
        where TService : class
    {
        private readonly List<ConfiguredProvider> configuredProviders;

        public ContextualizedServiceProviderGuilder(IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            Services = services;
            configuredProviders = new List<ConfiguredProvider>();
        }

        public IServiceCollection Services { get; }

        public void Add<TSpecialization>(TContext key, Action<TSpecialization> configure = null)
            where TSpecialization : TService
        {
            if (Services.All(descriptor => descriptor.ServiceType != typeof(TSpecialization)))
            {
                Services.AddTransient(typeof(TSpecialization));
            }
            configuredProviders.Add(new SpecializedConfiguredProvider<TSpecialization>(key, configure));
        }

        public IServiceCollection Build()
        {
            Services.AddTransient<ProviderGroup>();
            Services.AddSingleton(Create);
            return Services;
        }

        private IFactory<TContext, TService> CreateProviderFactory(IServiceProvider serviceProvider)
        {
            IFactoryBuilder<TContext, TService> builder = new FactoryBuilder<TContext, TService>();
            foreach (ConfiguredProvider configuredProvider in configuredProviders)
            {
                builder.Add<TContext, TService>(configuredProvider.Key, configuredProvider.InstanceType, serviceProvider, configuredProvider.Configure);
            }
            return builder.Build();
        }

        private IContextualizedServiceProvider<TContext, TService> Create(IServiceProvider serviceProvider)
        {
            var contextualizedServiceProvider = serviceProvider.GetRequiredService<ContextualizeIdServiceProvider<TContext, TService>>();
            providerGroup.Initialize(CreateProviderFactory(serviceProvider));
            return providerGroup;
        }

        private abstract class ConfiguredProvider
        {
            protected ConfiguredProvider(TContext key)
            {
                Key = key;
            }

            public TContext Key { get; }
            public abstract Type InstanceType { get; }
            public abstract void Configure(TService provider);
        }

        private class SpecializedConfiguredProvider<TSpecialization> : ConfiguredProvider
            where TSpecialization : TService
        {
            private readonly Action<TSpecialization> configureProvider;

            public SpecializedConfiguredProvider(TContext key, Action<TSpecialization> configure = null)
                : base(key)
            {
                configureProvider = configure;
            }

            public override Type InstanceType => typeof(TSpecialization);

            public override void Configure(TService provider)
            {
                configureProvider?.Invoke((TSpecialization)provider);
            }
        }
    }
}
