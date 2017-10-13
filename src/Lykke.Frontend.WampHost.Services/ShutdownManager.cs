using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Common.Log;
using Lykke.Frontend.WampHost.Core.Domain.Candles;
using Lykke.Frontend.WampHost.Core.Services;
using WampSharp.V2.MetaApi;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services
{
    public class ShutdownManager : IShutdownManager
    {
        private readonly ILog _log;
        private readonly IIndex<string, ICandlesSubscriber> _candlesSubscribers;
        private readonly IWampHostedRealm _realm;
        private readonly IHealthService _healthService;

        public ShutdownManager(
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

        public async Task StopAsync()
        {
            await _log.WriteInfoAsync(nameof(ShutdownManager), nameof(StopAsync), "", "Stopping spot candles subscriber...");

            _candlesSubscribers[nameof(MarketType.Spot)].Stop();

            await _log.WriteInfoAsync(nameof(ShutdownManager), nameof(StopAsync), "", "Stopping mt candles subscriber...");

            _candlesSubscribers[nameof(MarketType.Mt)].Stop();

            await _log.WriteInfoAsync(nameof(ShutdownManager), nameof(StopAsync), "", "Unsubscribing from realm sessions...");
            
            _realm.SessionCreated -= _healthService.TraceWampSessionCreated;
            _realm.SessionClosed -= _healthService.TraceWampSessionClosed;

            _realm.HostMetaApiService().Dispose();
        }
    }
}