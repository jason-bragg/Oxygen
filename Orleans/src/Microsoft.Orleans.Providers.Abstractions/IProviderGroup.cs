
using System;
using System.Collections.Generic;

namespace Microsoft.Orleans.Providers.Abstractions
{
    public interface IProviderGroup<TKey,TProvider>
        where TKey : IComparable<TKey>
        where TProvider : class
    {
        IReadOnlyCollection<KeyValuePair<TKey, TProvider>> Providers { get; }
    }
}
