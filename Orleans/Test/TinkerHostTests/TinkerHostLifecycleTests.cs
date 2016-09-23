
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Orleans.Host.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Orleans.Lifecycle.Abstractions;
using TinkerHost;
using Microsoft.Orleans.RingLifecycle.Abstractions;

namespace TinkerHostTests
{
    [TestClass]
    public class TinkerHostLifecycleTests
    {
        [TestMethod]
        public void SimpleTest()
        {
            IHostBuilder builder = TinkerHostBuilder.Create();
            IHost Host = builder.Build<Startup>();
            Host.Start();
            Host.Dispose();
        }

        [TestMethod]
        public void SimpleRingTest()
        {
            IHostBuilder builder = TinkerHostBuilder.Create();
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
            public Thingy(ILifecycleObservable lifecycle)
            {
                lifecycle.Subscribe(this);
            }

            public Task OnInitialize()
            {
                Console.WriteLine($"{GetType().Name} OnInitialize");
                return Task.FromResult(true);
            }

            public Task OnStart()
            {
                Console.WriteLine($"{GetType().Name} OnStart");
                return Task.FromResult(true);
            }

            public Task OnStop()
            {
                Console.WriteLine($"{GetType().Name} OnStop");
                return Task.FromResult(true);
            }
        }

        public class RingThingyBase : ILifecycleObserver
        {
            int ring;

            protected RingThingyBase(IRingLifecycleObservable lifecycle, int ring)
            {
                this.ring = ring;
                lifecycle.Subscribe(this, ring);
            }

            public Task OnInitialize()
            {
                Console.WriteLine($"{GetType().Name} OnInitialize, Ring: {ring}");
                return Task.FromResult(true);
            }

            public Task OnStart()
            {
                Console.WriteLine($"{GetType().Name} OnStart, Ring: {ring}");
                return Task.FromResult(true);
            }

            public Task OnStop()
            {
                Console.WriteLine($"{GetType().Name} OnStop, Ring: {ring}");
                return Task.FromResult(true);
            }
        }

        public class Thingy2 : RingThingyBase
        {
            public Thingy2(IRingLifecycleObservable lifecycle) : base(lifecycle, -10) {}
        }

        public class Thingy3 : RingThingyBase
        {
            public Thingy3(IRingLifecycleObservable lifecycle) : base(lifecycle, 10) { }
        }

        public class Thingy4 : RingThingyBase
        {
            public Thingy4(IRingLifecycleObservable lifecycle) : base(lifecycle, 100) { }
        }
    }
}
