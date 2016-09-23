
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Microsoft.Orleans.Host.Abstractions
{
    public interface IStartup
    {
        IEnumerable<Type> Members { get; }
        IServiceProvider ConfigureServices(IServiceCollection services);
    }
}