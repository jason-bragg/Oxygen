
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

        internal TinkerHost(IServiceCollection appServices, IServiceProvider hostingServiceProvider, IEnumerable<Type> members)
        {
            if (appServices == null) throw new ArgumentNullException(nameof(appServices));
            if (hostingServiceProvider == null) throw new ArgumentNullException(nameof(hostingServiceProvider));
            if (members == null) throw new ArgumentNullException(nameof(members));

            this.members = members.Select(type => hostingServiceProvider.GetService(type) ?? Activator.CreateInstance(type))
                                  .ToList();
            this.lifecycle = hostingServiceProvider.GetRequiredService<ILifecycleObserver>();
        }

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
