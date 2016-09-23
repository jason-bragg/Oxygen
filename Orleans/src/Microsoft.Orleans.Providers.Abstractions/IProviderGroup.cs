
using System.Collections.Generic;

namespace Microsoft.Orleans.Providers.Abstractions
{
    public interface IProviderGroup : IProvider
    {
        IReadOnlyCollection<KeyValuePair<string, IProvider>> Providers { get; }
    }
}
