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
using Lykke.RabbitMqBroker;
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
        private const string QueueName = "wamp";
        private const string TopicUri = "user.balances";

        public BalancesConsumer(
            [NotNull] ILog log,
            [NotNull] RabbitMqSettings settings,
            [NotNull] IWampHostedRealm realm,
            [NotNull] IClientResolver clientResolver)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _clientResolver = clientResolver ?? throw new ArgumentNullException(nameof(clientResolver));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            _subject = realm.Services.GetSubject(TopicUri);
        }

        public void Start()
        {
            try
            {
                var subscriptionSettings = new RabbitMqSubscriptionSettings
                {
                    ConnectionString = _settings.ConnectionString,
                    QueueName = $"{_settings.ExchangeName}.{QueueName}",
                    ExchangeName = _settings.ExchangeName,
                    IsDurable = false
                };
                _subscriber = new RabbitMqSubscriber<BalanceUpdateEventModel>(subscriptionSettings, new DefaultErrorHandlingStrategy(_log, subscriptionSettings))
                    .SetMessageDeserializer(new BalanceUpdateMqDeserializer())
                    .SetMessageReadStrategy(new MessageReadWithTemporaryQueueStrategy())
                    .Subscribe(Process)
                    .SetLogger(_log)
                    .Start();
            }
            catch (Exception ex)
            {
                _log.WriteError(nameof(BalancesConsumer), null, ex);
                throw;
            }
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

                try
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
                        Arguments = new object[] { new BalanceUpdateModel
                        {
                            Asset = balance.Asset,
                            NewBalance = balance.NewBalance,
                            NewReserved = balance.NewReserved,
                            OldBalance = balance.OldBalance,
                            OldReserved = balance.OldReserved
                        } }
                    });
                }
                catch (Exception ex)
                {
                    _log.WriteError(nameof(BalancesConsumer), message.ToJson(), ex);
                }
            }
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }
    }
}
