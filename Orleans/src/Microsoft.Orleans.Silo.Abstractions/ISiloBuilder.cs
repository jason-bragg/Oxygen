
using System;

namespace Microsoft.Orleans.Silo.Abstractions
{
    public interface ISiloBuilder
    {
        ISiloBuilder UseMessageCenter<TSiloMessageCenter>(Action<TSiloMessageCenter> configure = null)
            where TSiloMessageCenter : IMessageCenter;

        ISiloBuilder UseReminderService<TReminderService>(Action<TReminderService> configure = null)
            where TReminderService : IReminderService;

        ISiloBuilder UseMembershipOracle<TMembershipOracle>(Action<TMembershipOracle> configure = null)
            where TMembershipOracle : IMembershipOracle;

        ISilo Build();
    }
}
