using System;
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
        private const string TopicUri = "balances";

        public BalancesConsumer(
            [NotNull] ILog log,
            [NotNull] RabbitMqSettings settings,
            [NotNull] IWampHostedRealm realm,
            [NotNull] ISessionCache sessionCache,
            [NotNull] IRabbitMqSubscribeHelper rabbitMqSubscribeHelper)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _sessionCache = sessionCache ?? throw new ArgumentNullException(nameof(sessionCache));
            _rabbitMqSubscribeHelper = rabbitMqSubscribeHelper ?? throw new ArgumentNullException(nameof(rabbitMqSubscribeHelper));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

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

        private Task Process(BalanceUpdateEventModel message)
        {
            if (!message.Balances.Any())
                return Task.CompletedTask;

            foreach (var balance in message.Balances)
            {
                var sessionIds = _sessionCache.GetSessionIds(balance.Id);
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
                        Asset = balance.Asset,
                        Balance = balance.NewBalance,
                        Reserved = balance.NewReserved ?? default(double)
                    } }
                });
            }
            return Task.CompletedTask;
        }
    }
}
