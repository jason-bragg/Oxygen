
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Orleans.Providers.Default;
using Microsoft.Orleans.Silo.Abstractions;

namespace SampleProvider.Silo.Extensions
{
    public static class Extensions
    {
        public static ProviderGroupServiceBuilder<TKey, ISilo> AddDummySilo<TKey>(this ProviderGroupServiceBuilder<TKey, ISilo> builder, TKey key)
            where TKey : IComparable<TKey>
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.AddSingletonIfNotFound<IRuntime, DummyRuntime>()
                            .AddSingletonIfNotFound<IMembershipOracle, DummyMembershipOracle>()
                            .AddSingletonIfNotFound<IMessageCenter, DummyMessageCenter>()
                            .AddSingletonIfNotFound<IReminderService, DummyReminderService>();
            builder.Add<DummySilo>(key);
            return builder;
        }

        public static IServiceCollection AddSingletonIfNotFound<TService, TImplementation>(this IServiceCollection services)
            where TService : class where TImplementation : class, TService
        {
            return services.All(descriptor => descriptor.ServiceType != typeof(TService))
                ? services.AddSingleton<TService, TImplementation>()
                : services;
        }
    }
}
