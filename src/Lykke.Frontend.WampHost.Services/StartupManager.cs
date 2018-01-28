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
        private readonly IEnumerable<IWampHostedRealm> _realms;
        private readonly IHealthService _healthService;

        public StartupManager(
            ILog log,
            IEnumerable<ISubscriber> subscribers,
            IEnumerable<IWampHostedRealm> realms,
            IHealthService healthService)
        {
            _log = log;
            _subscribers = subscribers;
            _realms = realms;
            _healthService = healthService;
        }

        public async Task StartAsync()
        {
            await _log.WriteInfoAsync(nameof(StartupManager), nameof(StartAsync), "", "Subscribing to the realm sessions...");

            foreach (var realm in _realms)
            {
                realm.SessionCreated += _healthService.TraceWampSessionCreated;
                realm.SessionClosed += _healthService.TraceWampSessionClosed;
            }

            await _log.WriteInfoAsync(nameof(StartupManager), nameof(StartAsync), "", "Starting subscribers...");

            var tasks = _subscribers.Select(s => Task.Run(() => s.Start()));

            await Task.WhenAll(tasks);
        }
    }
}
