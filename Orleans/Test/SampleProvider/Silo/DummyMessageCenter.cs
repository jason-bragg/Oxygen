
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Orleans.Lifecycle.Abstractions;
using Microsoft.Orleans.Lifecycle.Abstractions.Extensions;
using Microsoft.Orleans.Silo.Abstractions;

namespace SampleProvider.Silo
{
    public class DummyMessageCenter : IMessageCenter
    {
        private readonly ILogger logger;

        public DummyMessageCenter(ILifecycleObservable lifecycle, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<DummyMessageCenter>();
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
