using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Services;
using WampSharp.V2.MetaApi;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services
{
    [UsedImplicitly]
    public class ShutdownManager : IShutdownManager
    {
        private readonly ILog _log;
        private readonly IWampHostedRealm _realm;
        private readonly IHealthService _healthService;

        public ShutdownManager(
            ILog log,
            IWampHostedRealm realm, 
            IHealthService healthService)
        {
            _log = log;
            _realm = realm;
            _healthService = healthService;
        }

        public async Task StopAsync()
        {
            await _log.WriteInfoAsync(nameof(ShutdownManager), nameof(StopAsync), "", "Unsubscribing from the realm sessions...");
            
            _realm.SessionCreated -= _healthService.TraceWampSessionCreated;
            _realm.SessionClosed -= _healthService.TraceWampSessionClosed;

            _realm.HostMetaApiService().Dispose();
        }
    }
}
