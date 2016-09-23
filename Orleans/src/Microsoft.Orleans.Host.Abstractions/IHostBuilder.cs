
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Orleans.Host.Abstractions
{
    public interface IHostBuilder
    {
        IHost Build<TStartup>() where TStartup : IStartup, new();

        IHostBuilder UseLoggerFactory(ILoggerFactory loggerFactory);

        IHostBuilder ConfigureMembers(IEnumerable<Type> members);

        IHostBuilder ConfigureServices(Action<IServiceCollection> configureServices);

        IHostBuilder ConfigureLogging(Action<ILoggerFactory> configureLogging);
    }
}