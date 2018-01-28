using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
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
        private IStopable _subscriber;
        private readonly IWampSubject _subject;
        private readonly IClientResolver _clientResolver;
        private readonly IRabbitMqSubscribersFactory _subscribersFactory;
        private const string TopicUri = "balances";

        public BalancesConsumer(
            [NotNull] ILog log,
            [NotNull] RabbitMqSettings settings,
            [NotNull] IWampHostedRealm realm,
            [NotNull] IClientResolver clientResolver,
            [NotNull] IRabbitMqSubscribersFactory subscribersFactory)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _clientResolver = clientResolver ?? throw new ArgumentNullException(nameof(clientResolver));
            _subscribersFactory = subscribersFactory ?? throw new ArgumentNullException(nameof(subscribersFactory));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            _subject = realm.Services.GetSubject(TopicUri);
        }

        public void Start()
        {
            _subscriber = _subscribersFactory.Create(
                connectionString: _settings.ConnectionString,
                source: "lykke.balanceupdate",
                deserializer: new JsonMessageDeserializer<BalanceUpdateEventModel>(),
                handler: Process);
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        private async Task Process(BalanceUpdateEventModel message)
        {
            if (!message.Balances.Any())
                return;

            foreach (var balance in message.Balances)
            {
                var notificationId = _clientResolver.GetNotificationId(balance.Id);

                if (notificationId == null)
                    continue;

                _subject.OnNext(new WampEvent
                {
                    Options = new PublishOptions
                    {
                        Eligible = new[] { long.Parse(notificationId) }
                    },
                    Arguments = new object[] { new BalanceUpdateMessage
                    {
                        Asset = balance.Asset,
                        Balance = balance.NewBalance,
                        Reserved = balance.NewReserved
                    } }
                });
            }
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }
    }
}
