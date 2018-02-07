using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
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
        private readonly RabbitMqSettings _settings;
        private readonly IWampSubject _subject;
        private readonly IClientResolver _clientResolver;
        private readonly IRabbitMqSubscribeHelper _rabbitMqSubscribeHelper;
        private const string TopicUri = "balances";

        public BalancesConsumer(
            [NotNull] ILog log,
            [NotNull] RabbitMqSettings settings,
            [NotNull] IWampHostedRealm realm,
            [NotNull] IClientResolver clientResolver,
            [NotNull] IRabbitMqSubscribeHelper rabbitMqSubscribeHelper)
        {
            _clientResolver = clientResolver ?? throw new ArgumentNullException(nameof(clientResolver));
            _rabbitMqSubscribeHelper =
                rabbitMqSubscribeHelper ?? throw new ArgumentNullException(nameof(rabbitMqSubscribeHelper));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            _subject = realm.Services.GetSubject(TopicUri);
        }

        public void Start()
        {
            _rabbitMqSubscribeHelper.Subscribe(
                connectionString: _settings.ConnectionString,
                source: "balanceupdate",
                deserializer: new JsonMessageDeserializer<BalanceUpdateEventModel>(),
                handler: Process, 
                market: MarketType.Spot);
        }

        private Task Process(BalanceUpdateEventModel message)
        {
            if (!message.Balances.Any())
                return Task.CompletedTask;

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
                        Reserved = balance.NewReserved ?? default(double)
                    } }
                });
            }
            
            return Task.CompletedTask;
        }
    }
}
