
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Orleans.Factory.Abstractions;
using Microsoft.Orleans.Factory.Abstractions.Extentions;
using Microsoft.Orleans.Factory.Default;
using Microsoft.Orleans.Host.Abstractions;
using Microsoft.Orleans.Silo.Abstractions;
using Microsoft.Orleans.StorageProvider.Abstractions;
using SampleProvider.Silo;
using SampleProvider.StorageProvider;
using TinkerHost;

namespace SampleProvider
{
    [TestClass]
    public class StorageProviderTests
    {
        [TestMethod]
        public void StorageProviderInGrainTest()
        {
            // create host
            IHostBuilder builder = TinkerHostBuilder.Create();
            builder.ConfigureLogging(factory => factory.AddProvider(new ConsoleLoggerProvider((s, l) => true, true)));
            IHost host = builder.Build<Startup>();

            // start host
            host.Start();

            // create grain
            SomePersistentGrain grain = host.Services.GetService<SomePersistentGrain>();
            grain.OnActivateAsync().Wait();
            grain.OnDeactivateAsync().Wait();

            // shutdown
            host.Dispose();
        }

        private class Startup : IStartup
        {
            // should probably get this from storage
            private static readonly KeyValuePair<string, string>[] configuredAzureProviders =
            {
                new KeyValuePair<string, string>("Azure", "secret"),
                new KeyValuePair<string, string>("Azure2", "secret2")
            };

            private readonly Type[] members;

            public Startup()
            {
                members = new[] { typeof(IStorageProviderGroup) };
            }

            public IEnumerable<Type> Members => members;

            public IServiceProvider ConfigureServices(IServiceCollection services)
            {
                // need to introduce config, but for now just configure in lambda
                services.AddSingleton(CreateTheProviderFactory);
                services.AddSingleton(CreateTheStorageProviderGroup);
                services.AddSingleton<ISiloRuntime, DummySiloRuntime>();
                services.AddTransient<SomePersistentGrain>();
                services.AddTransient<StorageProviderGroup>();
                services.AddTransient<AzureStorageProvider>();
                foreach (Type type in members.Where(t => services.All(descriptor => descriptor.ServiceType != t)))
                {
                    services.AddTransient(type);
                }
                return services.BuildServiceProvider();
            }

            private IFactory<string, IStorageProvider> CreateTheProviderFactory(IServiceProvider serviceProvider)
            {
                IFactoryBuilder<string, IStorageProvider> builder = new FactoryBuilder<string,IStorageProvider>();
                foreach (var kvp in configuredAzureProviders)
                {
                    builder.Add<string, IStorageProvider, AzureStorageProvider>(kvp.Key, serviceProvider, provider => provider.ConnectionString = kvp.Value);
                }
                return builder.Build();
            }
        
            private IStorageProviderGroup CreateTheStorageProviderGroup(IServiceProvider serviceProvider)
            {
                var group = serviceProvider.GetRequiredService<StorageProviderGroup>();
                group.ConfiguredProviders = configuredAzureProviders.Select(kvp => kvp.Key).ToList().AsReadOnly();
                return group;
            }
        }
    }
}
