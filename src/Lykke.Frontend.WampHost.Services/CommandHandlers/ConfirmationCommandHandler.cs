using System.Threading.Tasks;
using Common.Log;
using Lykke.Cqrs;
using Lykke.Frontend.WampHost.Contracts.Commands;
using Lykke.Frontend.WampHost.Core.Services.Operations;
using Lykke.Frontend.WampHost.Core.Services.Security;
using WampSharp.V2;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.CommandHandlers
{
    public class ConfirmationCommandHandler
    {
        private readonly IWampSubject _subject;
        private readonly ISessionCache _sessionCache;

        private const string Topic = "operations";

        public ConfirmationCommandHandler(
            IWampHostedRealm realm,
            ISessionCache sessionCache)
        {
            _subject = realm?.Services.GetSubject(Topic);
            _sessionCache = sessionCache;
        }

        public async Task<CommandHandlingResult> Handle(RequestConfirmationCommand command, IEventPublisher eventPublisher)
        {
            var sessionIds = _sessionCache.GetSessionIds(command.ClientId.ToString());
            if (sessionIds.Length == 0)
                return CommandHandlingResult.Ok();

            _subject.OnNext(new WampEvent
            {
                Options = new PublishOptions { Eligible = sessionIds },
                Arguments = new object[]
                {
                    new OperationStatusChangedMessage
                    {
                        Status = OperationStatus.ConfirmationRequested,
                        OperationId = command.OperationId
                    }
                }
            });

            return CommandHandlingResult.Ok();
        }
    }
}
