
using System;

namespace Microsoft.Orleans.Factory.Abstractions.Extentions
{
    public static class Extensions
    {
        public static void Add<TKey, TType, TSpecialization>(this IFactoryBuilder<TKey, TType> builder, TKey key, Action<TSpecialization> configure = null)
            where TKey : IComparable<TKey>
            where TType : class
            where TSpecialization : TType, new()
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Add(key, new InstanceFactory<TType, TSpecialization>(() => new TSpecialization(), configure));
        }

        public static void Add<TKey, TType, TSpecialization>(this IFactoryBuilder<TKey, TType> builder, TKey key, Func<TSpecialization> factoryFunc, Action<TSpecialization> configure = null)
            where TKey : IComparable<TKey>
            where TType : class
            where TSpecialization : TType
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (factoryFunc == null) throw new ArgumentNullException(nameof(factoryFunc));
            builder.Add(key, new InstanceFactory<TType,TSpecialization>(factoryFunc, configure));
        }

        public static void Add<TKey, TType, TSpecialization>(this IFactoryBuilder<TKey, TType> builder, TKey key, IServiceProvider serviceProvider, Action<TSpecialization> configure = null)
            where TKey : IComparable<TKey>
            where TType : class
            where TSpecialization : TType
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Add(key, new InstanceFactory<TType, TSpecialization>(() => (TSpecialization)serviceProvider.GetService(typeof(TSpecialization)), configure));
        }

        private class InstanceFactory<TType, TSpecialization> : IFactory<TType>
            where TType : class
            where TSpecialization : TType
        {
            private readonly Func<TSpecialization> factoryFunc;
            private readonly Action<TSpecialization> configAction;

            public InstanceFactory(Func<TSpecialization> factoryFunc, Action<TSpecialization> configAction)
            {
                this.factoryFunc = factoryFunc;
                this.configAction = configAction;
            }

            public TType Create()
            {
                TSpecialization instance = factoryFunc();
                configAction?.Invoke(instance);
                return instance;
            }
        }
    }
}
