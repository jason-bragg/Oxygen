﻿
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Orleans.Host.Abstractions;
using Microsoft.Orleans.Providers.Abstractions;
using Microsoft.Orleans.Providers.Default.Extensions;
using Microsoft.Orleans.Silo.Abstractions;
using Microsoft.Orleans.StorageProvider.Abstractions;
using SampleProvider.Silo;
using SampleProvider.StorageProvider;
using SampleProvider.StorageProvider.Extensions;
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
            private readonly Type[] members;

            public Startup()
            {
                members = new[]
                {
                    typeof(IProviderGroup<string,IStorageProvider>),
                    typeof(IProviderGroup<Guid,IStorageProvider>)
                };
            }

            public IEnumerable<Type> Members => members;

            public IServiceProvider ConfigureServices(IServiceCollection services)
            {
                services.AddSingleton<ISiloRuntime, DummySiloRuntime>()
                        .AddTransient<SomePersistentGrain>()
                        .AddProviderGroup<string,IStorageProvider>()
                            .AddAzureStorageProvider("Azure", "secret")
                            .AddAzureStorageProvider("Azure2", "secret2")
                        .Build()
                        .AddProviderGroup<Guid, IStorageProvider>()
                            .AddAzureStorageProvider(Guid.NewGuid(), "secret3")
                            .AddAzureStorageProvider(Guid.NewGuid(), "secret4")
                            .AddAzureStorageProvider(Guid.NewGuid(), "secret5")
                            .AddAzureStorageProvider(Guid.NewGuid(), "secret6")
                        .Build();
                foreach (Type type in members.Where(t => services.All(descriptor => descriptor.ServiceType != t)))
                {
                    services.AddTransient(type);
                }
                return services.BuildServiceProvider();
            }
        }
    }
}
