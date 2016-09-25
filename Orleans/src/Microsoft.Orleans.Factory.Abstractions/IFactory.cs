
using System;

namespace Microsoft.Orleans.Factory.Abstractions
{
    public interface IFactory<out TType>
        where TType : class
    {
        TType Create();
    }

    public interface IFactory<in TKey, out TType>
        where TKey : IComparable<TKey>
        where TType : class
    {
        TType Create(TKey key);
    }
}
