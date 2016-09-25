
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Orleans.Host.Abstractions;
using Microsoft.Orleans.Lifecycle.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace TinkerHost
{
    internal class TinkerHost : IHost
    {
        private readonly ILifecycleObserver lifecycle;
        private readonly List<object> members;

        internal TinkerHost(IServiceCollection appServices, IServiceProvider hostingServiceProvider, IEnumerable<Type> memberTypes)
        {
            if (appServices == null) throw new ArgumentNullException(nameof(appServices));
            if (hostingServiceProvider == null) throw new ArgumentNullException(nameof(hostingServiceProvider));
            if (memberTypes == null) throw new ArgumentNullException(nameof(memberTypes));

            Services = hostingServiceProvider;
            members = memberTypes.Select(type => hostingServiceProvider.GetService(type) ?? Activator.CreateInstance(type))
                                  .ToList();
            lifecycle = hostingServiceProvider.GetRequiredService<ILifecycleObserver>();
        }

        public IServiceProvider Services { get; }

        public void Start()
        {
            lifecycle.OnInitialize().Wait();
            lifecycle.OnStart().Wait();
        }

        public void Dispose()
        {
            lifecycle.OnStop().Wait();
        }
    }
}
