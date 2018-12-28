using System.Collections.Generic;
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
        private readonly IEnumerable<IWampHostedRealm> _realms;
        private readonly IHealthService _healthService;

        public ShutdownManager(
            ILog log,
            IEnumerable<ISubscriber> subscribers,
            IEnumerable<IWampHostedRealm> realms, 
            IHealthService healthService)
        {
            _log = log;
            _realms = realms;
            _healthService = healthService;
        }

        public Task StopAsync()
        {
            _log.WriteInfo(nameof(ShutdownManager), nameof(StopAsync), "Unsubscribing from the realms sessions...");

            foreach (var realm in _realms)
            {
                realm.SessionCreated -= _healthService.TraceWampSessionCreated;
                realm.SessionClosed -= _healthService.TraceWampSessionClosed;
                realm.HostMetaApiService().Dispose();
            }

            return Task.CompletedTask;
        }
    }
}
