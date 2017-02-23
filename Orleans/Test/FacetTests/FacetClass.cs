
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Orleans.Facet.Abstractions;
using Microsoft.Orleans.Factory.Abstractions;
using System;

namespace FacetTests
{
    [TestClass]
    public class FacetClass
    {
        [TestMethod]
        public void HappyPath()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IFactory<IHelloFacet>, InstanceFactory<IHelloFacet, HelloFacet>>();
            services.AddSingleton<IFactory<IEchoFacet>, InstanceFactory<IEchoFacet, EchoFacet>>();
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            TypeFacetInfo facetInfo = new TypeFacetInfo(typeof(Bob));
            Bob bob = new Bob();
            facetInfo.SetFields(bob, serviceProvider);

            bob.Hello.Hello();
            bob.Echo.Echo("blarg");
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
        private readonly IHelloFacet hello;
        private readonly IEchoFacet echo;

        public IHelloFacet Hello => hello;
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
}
