
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Orleans.Lifecycle.Abstractions;
using Microsoft.Orleans.Lifecycle.Abstractions.Extensions;
using Microsoft.Orleans.Silo.Abstractions;

namespace SampleProvider.Silo
{
    public class DummySilo : ISilo
    {
        private readonly ILogger logger;

        public DummySilo(
            ILifecycleObservable lifecycle,
            ILoggerFactory loggerFactory,
            IMembershipOracle membershipOracle,
            IReminderService reminderService,
            IMessageCenter messageCenter,
            IRuntime runtime)
        {
            logger = loggerFactory.CreateLogger<DummySilo>();
            lifecycle.Subscribe(Initialize, Start, Stop);
        }

        public Task Initialize()
        {
            logger.LogInformation("Initialize");
            return Task.FromResult(true);
        }

        public Task Start()
        {
            logger.LogInformation("Start");
            return Task.FromResult(true);
        }

        public Task Stop()
        {
            logger.LogInformation("Stop");
            return Task.FromResult(true);
        }
    }
}
