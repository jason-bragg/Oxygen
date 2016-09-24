
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Orleans.Host.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Orleans.Lifecycle.Abstractions;
using Microsoft.Orleans.RingLifecycle.Abstractions;
using TinkerHost;

namespace TinkerHostTests
{
    [TestClass]
    public class TinkerHostLifecycleTests
    {
        [TestMethod]
        public void SimpleTest()
        {
            IHostBuilder builder = TinkerHostBuilder.Create();
            builder.ConfigureLogging(factory => factory.AddProvider(new ConsoleLoggerProvider((s, l) => true, true)));
            IHost Host = builder.Build<Startup>();
            Host.Start();
            Host.Dispose();
        }

        [TestMethod]
        public void SimpleRingTest()
        {
            IHostBuilder builder = TinkerHostBuilder.Create();
            builder.ConfigureLogging(factory => factory.AddProvider(new ConsoleLoggerProvider((s, l) => true, true)));
            IHost Host = builder.Build<Startup2>();
            Host.Start();
            Host.Dispose();
        }

        private class Startup : IStartup
        {
            private readonly Type[] members;
            public Startup()
            {
                members = new[] { typeof(Thingy) };
            }

            public IEnumerable<Type> Members => members;

            public IServiceProvider ConfigureServices(IServiceCollection services)
            {
                foreach(Type type in members)
                {
                    services.AddTransient(type);
                }
                return services.BuildServiceProvider();
            }
        }

        private class Startup2 : IStartup
        {
            private readonly Type[] members;
            public Startup2()
            {
                members = new[] { typeof(Thingy), typeof(Thingy2), typeof(Thingy3), typeof(Thingy4) };
            }

            public IEnumerable<Type> Members => members;

            public IServiceProvider ConfigureServices(IServiceCollection services)
            {
                services.AddSingleton<IRingLifecycleObservable, Microsoft.Orleans.RingLifecycle.Default.Lifecycle>();
                foreach (Type type in members)
                {
                    services.AddTransient(type);
                }
                return services.BuildServiceProvider();
            }
        }

        public class Thingy : ILifecycleObserver
        {
            private readonly ILogger logger;
            public Thingy(ILifecycleObservable lifecycle, ILoggerFactory loggerFactory)
            {
                logger = loggerFactory.CreateLogger<Thingy>();
                lifecycle.Subscribe(this);
            }

            public Task OnInitialize()
            {
                logger.LogInformation("OnInitialize");
                return Task.FromResult(true);
            }

            public Task OnStart()
            {
                logger.LogInformation("OnStart");
                return Task.FromResult(true);
            }

            public Task OnStop()
            {
                logger.LogInformation("OnStop");
                return Task.FromResult(true);
            }
        }

        public class RingThingyBase : ILifecycleObserver
        {
            private readonly ILogger logger;
            private readonly int ring;

            protected RingThingyBase(IRingLifecycleObservable lifecycle, ILoggerFactory loggerFactory, int ring)
            {
                logger = loggerFactory.CreateLogger(GetType());
                this.ring = ring;
                lifecycle.Subscribe(this, ring);
            }

            public Task OnInitialize()
            {
                logger.LogInformation("OnInitialize. Ring: {0}", ring);
                return Task.FromResult(true);
            }

            public Task OnStart()
            {
                logger.LogInformation("OnStart. Ring: {0}", ring);
                return Task.FromResult(true);
            }

            public Task OnStop()
            {
                logger.LogInformation("OnStop. Ring: {0}", ring);
                return Task.FromResult(true);
            }
        }

        public class Thingy2 : RingThingyBase
        {
            public Thingy2(IRingLifecycleObservable lifecycle, ILoggerFactory loggerFactory) : base(lifecycle, loggerFactory, -10) {}
        }

        public class Thingy3 : RingThingyBase
        {
            public Thingy3(IRingLifecycleObservable lifecycle, ILoggerFactory loggerFactory) : base(lifecycle, loggerFactory, 10) { }
        }

        public class Thingy4 : RingThingyBase
        {
            public Thingy4(IRingLifecycleObservable lifecycle, ILoggerFactory loggerFactory) : base(lifecycle, loggerFactory, 100) { }
        }
    }
}
