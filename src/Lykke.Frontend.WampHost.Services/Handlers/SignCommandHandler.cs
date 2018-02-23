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
using Newtonsoft.Json.Linq;
using WampSharp.V2;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Realm;
using Lykke.Frontend.WampHost.Services.Security;

namespace Lykke.Frontend.WampHost.Services.Handlers
{
    public class SignCommandHandler
    {
        private readonly ILog _log;        
        private readonly ISessionCache _sessionCache;
        private readonly IWampSubject _subject;

        public SignCommandHandler(ILog log, IWampHostedRealm realm, ISessionCache sessionCache)
        {
            _log = log;            
            _sessionCache = sessionCache;

            _subject = realm.Services.GetSubject("commands.sign");
        }

        public async Task<CommandHandlingResult> Handle(SignCommand signCommand)
        {
            try
            {
                var sessionIds = _sessionCache.GetSessionIds(signCommand.ClientId);

                if (sessionIds == null)
                    return CommandHandlingResult.Ok();

                _subject.OnNext(new WampEvent
                {
                    Options = new PublishOptions
                    {
                        Eligible = sessionIds   
                    },
                    Arguments = new object[] { new { signCommand.RequestId, signCommand.RequestType, Context = JObject.Parse(signCommand.Context) } }
                });                
            }
            catch (Exception ex)
            {
                _log.WriteWarning(nameof(SignCommandHandler), signCommand.ClientId, "Failed to process sign command", ex);                                
            }
            
            return CommandHandlingResult.Ok();
        }
    }
}
