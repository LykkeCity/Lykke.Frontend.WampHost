using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Services;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services
{
    [UsedImplicitly]
    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly IEnumerable<ISubscriber> _subscribers;
        private readonly IWampHostedRealm _realm;
        private readonly IHealthService _healthService;

        public StartupManager(
            ILog log, 
            IEnumerable<ISubscriber> subscribers,
            IWampHostedRealm realm,
            IHealthService healthService)
        {
            _log = log;
            _subscribers = subscribers;
            _realm = realm;
            _healthService = healthService;
        }

        public async Task StartAsync()
        {
            await _log.WriteInfoAsync(nameof(StartupManager), nameof(StartAsync), "", "Subscribing to the realm sessions...");
            
            _realm.SessionCreated += _healthService.TraceWampSessionCreated;
            _realm.SessionClosed += _healthService.TraceWampSessionClosed;
            
            await _log.WriteInfoAsync(nameof(StartupManager), nameof(StartAsync), "", "Starting subscribers...");

            var tasks = _subscribers.Select(s => Task.Run(() => s.Start()));

            await Task.WhenAll(tasks);
        }
    }
}
