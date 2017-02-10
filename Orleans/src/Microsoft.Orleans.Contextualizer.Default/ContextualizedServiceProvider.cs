
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Orleans.Contextualizer.Abstractions;
using Microsoft.Orleans.Factory.Abstractions;

namespace Microsoft.Orleans.Contextualizer.Default
{
    public class ConfigurationContextualizer<TContext, TService>
        where TContext : IComparable<TContext>
        where TService : class
    {
        public ConfigurationContextualizer(IConfiguration config)
        {

        }
    }

    public class ContextualizedServiceProvider<TContext, TService> : IContextualizedServiceProvider<TContext,TService>
        where TContext : IComparable<TContext>
        where TService : class
    {
        private readonly ILogger logger;

        public ContextualizedServiceProvider(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

            logger = loggerFactory.CreateLogger<ContextualizedServiceProvider<TContext, TService>>();
        }

        public void Initialize(IFactory<TContext, TService> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            logger.LogInformation("Initialize");

            Providers = factory.Keys.ToDictionary(key => key, factory.Create);
        }

        public TService GetService(TContext key)
        {
            throw new NotImplementedException();
        }
    }
}
