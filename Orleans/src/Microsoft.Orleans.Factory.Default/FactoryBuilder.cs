
using System;
using System.Collections.Generic;
using Microsoft.Orleans.Factory.Abstractions;
using System.Linq;

namespace Microsoft.Orleans.Factory.Default
{
    public class FactoryBuilder<TKey, TType> : IFactoryBuilder<TKey, TType>
        where TKey : IComparable<TKey>
        where TType : class
    {
        private readonly IDictionary<TKey, IFactory<TType>> factoryMap;

        public FactoryBuilder()
        {
            factoryMap = new Dictionary<TKey, IFactory<TType>>();
        }

        public void Add(TKey key, IFactory<TType> factory)
        {
            if(factory == null) throw new ArgumentNullException(nameof(factory));
            factoryMap.Add(key, factory);
        }

        public IFactory<TKey, TType> Build()
        {
            return new Factory(factoryMap);
        }

        private class Factory : IFactory<TKey, TType>
        {
            private readonly IDictionary<TKey, IFactory<TType>> factoryMap;

            public Factory(IDictionary<TKey, IFactory<TType>> map)
            {
                factoryMap = map;
            }

            public IReadOnlyCollection<TKey> Keys => factoryMap.Keys.ToList().AsReadOnly();

            public TType Create(TKey key)
            {
                IFactory<TType> factory;
                return factoryMap.TryGetValue(key, out factory)
                    ? factory.Create()
                    : default(TType);
            }
        }
    }
}
