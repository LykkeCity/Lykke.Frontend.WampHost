using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Cqrs;
using Lykke.Frontend.WampHost.Core.Services.Orderbook;
using Lykke.Frontend.WampHost.Core.Services.Security;
using Lykke.Frontend.WampHost.Services.Balances.Contracts;
using Lykke.Frontend.WampHost.Services.Commands;
using Newtonsoft.Json;
using WampSharp.V2;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Handlers
{
    public class SignCommandHandler
    {
        private readonly ILog _log;        
        private readonly IClientResolver _clientResolver;
        private readonly IWampSubject _subject;

        public SignCommandHandler(ILog log, IWampHostedRealm realm, IClientResolver clientResolver)
        {
            _log = log;            
            _clientResolver = clientResolver;

            _subject = realm.Services.GetSubject("commands.sign");
        }

        public async Task<CommandHandlingResult> Handle(SignCommand signCommand)
        {
            try
            {
                var notificationId = _clientResolver.GetNotificationId(signCommand.ClientId);

                if (notificationId == null)
                    return CommandHandlingResult.Ok();

                _subject.OnNext(new WampEvent
                {
                    Options = new PublishOptions
                    {
                        Eligible = new[] { long.Parse(notificationId) }
                    },
                    Arguments = new object[] { signCommand }
                });                
            }
            catch (Exception)
            {
                _log.WriteWarning(nameof(SignCommandHandler), signCommand.ClientId, "Failed to process sign command");                                
            }
            
            return CommandHandlingResult.Ok();
        }
    }
}
