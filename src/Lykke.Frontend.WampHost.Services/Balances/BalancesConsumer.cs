using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Security;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.Balances.Contracts;
using Lykke.Frontend.WampHost.Services.Balances.IncomeMessages;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.ClientAccount.Client.AutorestClient.Models;
using WampSharp.V2;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Balances
{
    [UsedImplicitly]
    public class BalancesConsumer : ISubscriber
    {
        private readonly ILog _log;
        private readonly RabbitMqSettings _settings;
        private readonly IWampSubject _subject;
        private readonly ISessionCache _sessionCache;
        private readonly IRabbitMqSubscribeHelper _rabbitMqSubscribeHelper;
        private readonly IClientAccountClient _clientAccountClient;
        private const string TopicUri = "balances";
        private readonly Dictionary<string, string> _clientIdsMappingsToTradingWalletIds;

        public BalancesConsumer(
            [NotNull] ILog log,
            [NotNull] RabbitMqSettings settings,
            [NotNull] IWampHostedRealm realm,
            [NotNull] ISessionCache sessionCache,
            [NotNull] IClientAccountClient clientAccountClient,
            [NotNull] IRabbitMqSubscribeHelper rabbitMqSubscribeHelper)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _sessionCache = sessionCache ?? throw new ArgumentNullException(nameof(sessionCache));
            _rabbitMqSubscribeHelper = rabbitMqSubscribeHelper ?? throw new ArgumentNullException(nameof(rabbitMqSubscribeHelper));
            _clientAccountClient = clientAccountClient ?? throw new ArgumentNullException(nameof(clientAccountClient));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            _subject = realm.Services.GetSubject(TopicUri);
            
            _clientIdsMappingsToTradingWalletIds = new Dictionary<string, string>();
        }

        public void Start()
        {
            _rabbitMqSubscribeHelper.Subscribe(
                connectionString: _settings.ConnectionString,
                market: MarketType.Spot,
                source: "balanceupdate",
                deserializer: new JsonMessageDeserializer<BalanceUpdateEventModel>(),
                handler: Process);
        }

        private async Task Process(BalanceUpdateEventModel message)
        {
            if (!message.Balances.Any())
                return;

            var idsMappings = new Dictionary<string, string>();

            foreach (var balance in message.Balances)
            {
                var meWalletId = balance.Id;

                if (!idsMappings.ContainsKey(meWalletId))
                    idsMappings[meWalletId] = await _clientAccountClient.GetClientByWalletAsync(meWalletId);

                var clientId = idsMappings[meWalletId];

                string lykkeWalletId;

                if (clientId == meWalletId)
                {
                    if (!_clientIdsMappingsToTradingWalletIds.ContainsKey(clientId))
                    {
                        _clientIdsMappingsToTradingWalletIds[clientId] =
                            (await _clientAccountClient.GetClientWalletsByTypeAsync(clientId, WalletType.Trading)).First().Id;
                    }
                    
                    lykkeWalletId = _clientIdsMappingsToTradingWalletIds[clientId];
                }
                else
                {
                    lykkeWalletId = meWalletId;
                }

                var sessionIds = _sessionCache.GetSessionIds(clientId);
                if (sessionIds.Length == 0)
                    continue;

                _subject.OnNext(new WampEvent
                {
                    Options = new PublishOptions
                    {
                        Eligible = sessionIds
                    },
                    Arguments = new object[] { new BalanceUpdateMessage
                    {
                        WalletId = lykkeWalletId,
                        Asset = balance.Asset,
                        Balance = balance.NewBalance,
                        Reserved = balance.NewReserved ?? default(double)
                    } }
                });
            }
        }
    }
}
