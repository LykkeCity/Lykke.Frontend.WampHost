using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Security;
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
        private readonly ISessionCache _sessionCache;

        public StartupManager(
            ILog log,
            IEnumerable<ISubscriber> subscribers,
            IEnumerable<IWampHostedRealm> realms,
            IHealthService healthService,
            ISessionCache sessionCache)
        {
            _log = log;
            _subscribers = subscribers;
            _realms = realms;
            _healthService = healthService;
            _sessionCache = sessionCache;
        }

        public async Task StartAsync()
        {
            _log.WriteInfo(nameof(StartAsync), "", "Subscribing to the realm sessions...");

            foreach (var realm in _realms)
            {
                realm.SessionCreated += _healthService.TraceWampSessionCreated;
                realm.SessionClosed += _healthService.TraceWampSessionClosed;
                realm.SessionClosed += (sender, args) => { _sessionCache.TryRemoveSessionId(args.SessionId); };
            }

            _log.WriteInfo(nameof(StartAsync), "", "Starting subscribers...");

            var tasks = _subscribers.Select(s => Task.Run(() => s.Start()));

            await Task.WhenAll(tasks);
        }
    }
}
