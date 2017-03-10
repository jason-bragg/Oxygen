
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Orleans.Facet.Abstractions;
using Microsoft.Orleans.Factory.Abstractions;
using Microsoft.Orleans.Factory.Default;
using Microsoft.Orleans.Factory.Abstractions.Extentions;

namespace FacetTests
{
    [TestClass]
    public class FacetClassTests
    {
        [TestMethod]
        public void HappyFieldPath()
        {
            IServiceCollection services = new ServiceCollection();
            IFactoryBuilder<string, IHelloFacet> builder = new FactoryBuilder<string, IHelloFacet>();
            builder.Add<string, IHelloFacet, HelloFacet>("h1");
            builder.Add<string, IHelloFacet, HelloFacet2>("h2");
            services.AddSingleton<IFactory<string, IHelloFacet>>((sp) => builder.Build());
            services.AddSingleton<IFactory<IEchoFacet>, InstanceFactory<IEchoFacet, EchoFacet>>();
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            TypeFacetInfo facetInfo = new TypeFacetInfo(typeof(Bob));
            Bob bob = new Bob();
            facetInfo.SetFields(bob, serviceProvider);

            bob.Hello1.Hello();
            bob.Hello2.Hello();
            bob.Echo.Echo("blarg");
        }

        [TestMethod]
        public void HappyConstructorPath()
        {
            IServiceCollection services = new ServiceCollection();
            IFactoryBuilder<string, IHelloFacet> builder = new FactoryBuilder<string, IHelloFacet>();
            builder.Add<string, IHelloFacet, HelloFacet>("h1");
            builder.Add<string, IHelloFacet, HelloFacet2>("h2");
            services.AddSingleton<IFactory<string, IHelloFacet>>((sp) => builder.Build());
            services.AddSingleton<IFactory<IEchoFacet>, InstanceFactory<IEchoFacet, EchoFacet>>();
            services.AddTransient<Fidget>();
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            TypeFacetInfo facetInfo = new TypeFacetInfo(typeof(Bob2));
            object[] facets = facetInfo.CreateConstructorParameterFacets(serviceProvider);
            ObjectFactory factory = ActivatorUtilities.CreateFactory(typeof(Bob2), facets.Select(f => f.GetType()).ToArray());
            Bob2 bob = factory.Invoke(serviceProvider, facets) as Bob2;

            bob.Hello1.Hello();
            bob.Hello2.Hello();
            bob.Echo.Echo("blarg");
            bob.Fidget.Dance();
        }

    }

    public interface IHelloFacet : IGrainFacet
    {
        void Hello();
    }

    public class HelloFacet : IHelloFacet
    {
        public void Hello()
        {
            Console.WriteLine("Hello");
        }
    }

    public class HelloFacet2 : IHelloFacet
    {
        public void Hello()
        {
            Console.WriteLine("Hello2");
        }
    }

    public interface IEchoFacet : IGrainFacet
    {
        void Echo(string phrase);
    }

    public class EchoFacet : IEchoFacet
    {
        public void Echo(string phrase)
        {
            Console.WriteLine(phrase);
        }
    }

    public class Bob
    {
        [Facet("h1")]
        private readonly IHelloFacet hello1;
        [Facet("h2")]
        private readonly IHelloFacet hello2;
        private readonly IEchoFacet echo;

        public IHelloFacet Hello1 => hello1;
        public IHelloFacet Hello2 => hello2;
        public IEchoFacet Echo => echo;
    }

    class InstanceFactory<TType, TSpecialization> : IFactory<TType>
            where TType : class
            where TSpecialization : TType, new()
    {
        public TType Create()
        {
            return new TSpecialization();
        }
    }

    public interface ITransactionMonitor<TState>
    {
        Task OnCompleted(TState state);
        Task OnAborted(TState state);
    }

    public interface ITransactionalState<TState> : IGrainFacet
    {
        TState State { get; }
        ITransactionMonitor<TState> Monitor { set; }
        void Save();
    }
    public class Bob2
    {
        public Bob2()
        { }

        [FacetConstructor]
        public Bob2(
            [Facet("h1")]
            IHelloFacet hello1,
            [Facet("h2")]
            IHelloFacet hello2,
            IEchoFacet echo,
            Fidget fidget)
        {
            this.Hello1 = hello1;
            this.Hello2 = hello2;
            this.Echo = echo;
            this.Fidget = fidget;
        }
        
        public IHelloFacet Hello1 { get; }

        public IHelloFacet Hello2 { get; }

        public IEchoFacet Echo { get; }

        public Fidget Fidget { get; set; }
    }

    public class Fidget
    {
        public void Dance()
        {
            Console.WriteLine("Dance!");
        }

    }
}
