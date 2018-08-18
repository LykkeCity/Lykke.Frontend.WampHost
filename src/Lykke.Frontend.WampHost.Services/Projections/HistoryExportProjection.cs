using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Services.HistoryExport;
using Lykke.Frontend.WampHost.Core.Services.Security;
using Lykke.Job.HistoryExportBuilder.Contract.Events;
using WampSharp.V2;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Projections
{
    [UsedImplicitly]
    public class HistoryExportProjection
    {
        private readonly ILog _log;
        private readonly IWampSubject _subject;
        private readonly ISessionCache _sessionCache;
        
        private const string Topic = "history.export";

        public HistoryExportProjection(
            ILog log,
            IWampHostedRealm realm,
            ISessionCache sessionCache)
        {
            _log = log;
            _subject = realm?.Services.GetSubject(Topic);
            _sessionCache = sessionCache;
        }
        
        public Task Handle(ClientHistoryExportedEvent evt)
        {
            var sessionIds = _sessionCache.GetSessionIds(evt.ClientId);
            if (sessionIds.Length == 0)
                return Task.CompletedTask;
            
            _subject.OnNext(new WampEvent
            {
                Options = new PublishOptions { Eligible = sessionIds },
                Arguments = new object[] { new HistoryExportGeneratedMessage { Id = evt.Id, Url = evt.Uri} }
            });

            return Task.CompletedTask;
        }
    }
}
