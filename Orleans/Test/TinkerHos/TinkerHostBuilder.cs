
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Orleans.Host.Abstractions;
using Microsoft.Orleans.Lifecycle.Default;
using Microsoft.Orleans.Lifecycle.Abstractions;

namespace TinkerHost
{
    public class TinkerHostBuilder : IHostBuilder
    {
        private readonly List<Action<IServiceCollection>> configureServicesDelegates;
        private readonly List<Action<ILoggerFactory>> configureLoggingDelegates;
        private readonly ISet<Type> members;

        private ILoggerFactory loggerFactory;

        public static IHostBuilder Create()
        {
            return new TinkerHostBuilder();
        }

        public IHostBuilder UseLoggerFactory(ILoggerFactory logFactory)
        {
            if (logFactory == null) throw new ArgumentNullException(nameof(logFactory));
            loggerFactory = logFactory;
            return this;
        }

        public IHostBuilder ConfigureMembers(IEnumerable<Type> newNembers)
        {
            foreach (Type type in newNembers)
            {
                members.Add(type);
            }
            return this;
        }

        public IHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            if (configureServices == null) throw new ArgumentNullException(nameof(configureServices));
            configureServicesDelegates.Add(configureServices);
            return this;
        }

        public IHostBuilder ConfigureLogging(Action<ILoggerFactory> configureLogging)
        {
            if (configureLogging == null) throw new ArgumentNullException(nameof(configureLogging));
            configureLoggingDelegates.Add(configureLogging);
            return this;
        }

        public IHost Build<TStartup>() where TStartup : IStartup, new()
        {
            IStartup startup = new TStartup();
            ConfigureMembers(startup.Members);
            IServiceCollection hostingServices = BuildHostingServices();
            startup.ConfigureServices(hostingServices);
            IServiceProvider hostingContainer = startup.ConfigureServices(hostingServices);
            return new TinkerHost(hostingServices, hostingContainer, members);
        }

        private IServiceCollection BuildHostingServices()
        {
            var services = new ServiceCollection();

            var lifecycle = new Lifecycle();
            services.AddSingleton<ILifecycleObserver>(sp => lifecycle);
            services.AddSingleton<ILifecycleObservable>(sp => lifecycle);

            if (loggerFactory == null)
            {
                loggerFactory = new LoggerFactory();
            }

            foreach (var configureLogging in configureLoggingDelegates)
            {
                configureLogging(loggerFactory);
            }

            services.AddSingleton(loggerFactory);
            services.AddLogging();
            foreach (var configureServices in configureServicesDelegates)
            {
                configureServices(services);
            }

            return services;
        }

        private TinkerHostBuilder()
        {
            configureServicesDelegates = new List<Action<IServiceCollection>>();
            configureLoggingDelegates = new List<Action<ILoggerFactory>>();
            members = new HashSet<Type>();
        }
    }
}
