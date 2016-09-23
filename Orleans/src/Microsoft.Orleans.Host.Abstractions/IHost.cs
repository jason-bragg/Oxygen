
using System;

namespace Microsoft.Orleans.Host.Abstractions
{
    public interface IHost : IDisposable
    {
        void Start();
    }
}
