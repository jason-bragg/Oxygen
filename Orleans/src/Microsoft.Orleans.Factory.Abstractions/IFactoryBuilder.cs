
using System;

namespace Microsoft.Orleans.Factory.Abstractions
{
    public interface IFactoryBuilder<in TKey, TType>
        where TKey : IComparable<TKey>
        where TType : class
    {
        void Add(TKey key, IFactory<TType> factory);
        IFactory<TKey, TType> Build();
    }
}
