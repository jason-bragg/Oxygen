
using System;

namespace Microsoft.Orleans.Host.Abstractions
{
    public interface IHost : IDisposable
    {
        IServiceProvider Services { get; }
        void Start();
    }
}
