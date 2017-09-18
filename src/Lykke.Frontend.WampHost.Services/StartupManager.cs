using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Common.Log;
using Lykke.Frontend.WampHost.Core.Services;

namespace Lykke.Frontend.WampHost.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly IIndex<string, ICandlesSubscriber> _candlesSubscribers;

        public StartupManager(ILog log, IIndex<string, ICandlesSubscriber> candlesSubscribers)
        {
            _log = log;
            _candlesSubscribers = candlesSubscribers;
        }

        public async Task StartAsync()
        {
            await _log.WriteInfoAsync(nameof(StartupManager), nameof(StartAsync), "", "Starting spot candles subscriber...");

            _candlesSubscribers["spot"].Start();

            await _log.WriteInfoAsync(nameof(StartupManager), nameof(StartAsync), "", "Starting mt candles subscriber...");

            _candlesSubscribers["mt"].Start();
        }
    }

    public interface IStartupManager
    {
        Task StartAsync();
    }
}