using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Common.Log;
using Lykke.Frontend.WampHost.Core.Domain.Candles;
using Lykke.Frontend.WampHost.Core.Services;
using WampSharp.V2;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly IIndex<string, ICandlesSubscriber> _candlesSubscribers;
        private readonly IWampHostedRealm _realm;
        private readonly IHealthService _healthService;

        public StartupManager(
            ILog log, 
            IIndex<string, ICandlesSubscriber> candlesSubscribers,
            IWampHostedRealm realm,
            IHealthService healthService)
        {
            _log = log;
            _candlesSubscribers = candlesSubscribers;
            _realm = realm;
            _healthService = healthService;
        }

        public async Task StartAsync()
        {
            await _log.WriteInfoAsync(nameof(StartupManager), nameof(StartAsync), "", "Subscribing to realm sessions...");
            
            _realm.SessionCreated += _healthService.TraceWampSessionCreated;
            _realm.SessionClosed += _healthService.TraceWampSessionClosed;
            
            await _log.WriteInfoAsync(nameof(StartupManager), nameof(StartAsync), "", "Starting spot candles subscriber...");

            _candlesSubscribers[nameof(MarketType.Spot)].Start();

            await _log.WriteInfoAsync(nameof(StartupManager), nameof(StartAsync), "", "Starting mt candles subscriber...");

            _candlesSubscribers[nameof(MarketType.Mt)].Start();
        }
    }
}