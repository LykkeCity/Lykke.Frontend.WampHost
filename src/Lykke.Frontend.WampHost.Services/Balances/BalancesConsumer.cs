using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Clients;
using Lykke.Frontend.WampHost.Core.Services.Security;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.Balances.Contracts;
using Lykke.Frontend.WampHost.Services.Balances.IncomeMessages;
using Lykke.RabbitMqBroker.Subscriber;
using WampSharp.V2;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Balances
{
    [UsedImplicitly]
    public class BalancesConsumer : ISubscriber
    {
        private const string TopicUri = "balances";

        private readonly ILog _log;
        private readonly RabbitMqSettings _settings;
        private readonly IWampSubject _subject;
        private readonly ISessionCache _sessionCache;
        private readonly IClientToWalletMapper _clientToWalletMapper;
        private readonly IRabbitMqSubscribeHelper _rabbitMqSubscribeHelper;

        public BalancesConsumer(
            [NotNull] ILog log,
            [NotNull] RabbitMqSettings settings,
            [NotNull] IWampHostedRealm realm,
            [NotNull] ISessionCache sessionCache,
            [NotNull] IClientToWalletMapper clientToWalletMapper,
            [NotNull] IRabbitMqSubscribeHelper rabbitMqSubscribeHelper)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _sessionCache = sessionCache ?? throw new ArgumentNullException(nameof(sessionCache));
            _rabbitMqSubscribeHelper = rabbitMqSubscribeHelper ?? throw new ArgumentNullException(nameof(rabbitMqSubscribeHelper));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _clientToWalletMapper = clientToWalletMapper ?? throw new ArgumentNullException(nameof(clientToWalletMapper));
            _subject = realm.Services.GetSubject(TopicUri);
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

            try
            {
                foreach (var balance in message.Balances)
                {
                    var meWalletId = balance.Id;

                    var (clientId, walletId) = await _clientToWalletMapper.GetClientIdAndWalletIdAsync(meWalletId);

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
                            WalletId = walletId,
                            Asset = balance.Asset,
                            Balance = balance.NewBalance,
                            Reserved = balance.NewReserved ?? default(double)
                        } }
                    });
                }
            }
            catch (Exception e)
            {
                _log.WriteError(nameof(BalancesConsumer), message, e);
                throw;
            }
        }
    }
}
