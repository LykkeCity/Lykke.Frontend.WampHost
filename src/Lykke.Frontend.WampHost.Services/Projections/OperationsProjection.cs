using System;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Services.HistoryExport;
using Lykke.Frontend.WampHost.Core.Services.Operations;
using Lykke.Frontend.WampHost.Core.Services.Security;
using Lykke.Job.HistoryExportBuilder.Contract.Events;
using Lykke.Service.Operations.Contracts.Events;
using WampSharp.V2;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Projections
{
    [UsedImplicitly]
    public class OperationsProjection
    {
        private readonly IWampSubject _subject;
        private readonly ISessionCache _sessionCache;

        private const string Topic = "operations";

        public OperationsProjection(
            IWampHostedRealm realm,
            ISessionCache sessionCache)
        {
            _subject = realm?.Services.GetSubject(Topic);
            _sessionCache = sessionCache;
        }

        public Task Handle(OperationConfirmedEvent evt)
        {
            SendOperationStatus(evt.ClientId, evt.OperationId, OperationStatus.Confirmed);

            return Task.CompletedTask;
        }

        public Task Handle(OperationCompletedEvent evt)
        {
            SendOperationStatus(evt.ClientId, evt.OperationId, OperationStatus.Completed);

            return Task.CompletedTask;
        }

        public Task Handle(OperationFailedEvent evt)
        {
            SendOperationStatus(evt.ClientId, evt.OperationId, OperationStatus.Failed, evt.ErrorCode, evt.ErrorMessage);

            return Task.CompletedTask;
        }

        public Task Handle(OperationCorruptedEvent evt)
        {
            SendOperationStatus(evt.ClientId, evt.OperationId, OperationStatus.Failed);

            return Task.CompletedTask;
        }

        private void SendOperationStatus(Guid clientId, Guid operationId, OperationStatus status,
            string errorCode = null, string errorMessage = null)
        {
            var sessionIds = _sessionCache.GetSessionIds(clientId.ToString());
            if (sessionIds.Length == 0)
                return;

            _subject.OnNext(new WampEvent
            {
                Options = new PublishOptions { Eligible = sessionIds },
                Arguments = new object[]
                {
                    new OperationStatusChangedMessage
                    {
                        Status = status,
                        OperationId = operationId,
                        ErrorCode = errorCode,
                        ErrorMessage = errorMessage
                    }
                }
            });
        }
    }
}
