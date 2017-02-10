
using System;
using System.Collections.Generic;

namespace Microsoft.Orleans.Factory.Abstractions
{
    public interface IFactory<out TType>
        where TType : class
    {
        TType Create();
    }

    public interface IFactory<TKey, out TType>
        where TKey : IComparable<TKey>
        where TType : class
    {
        IReadOnlyCollection<TKey> Keys { get; }
        TType Create(TKey key);
    }
}
