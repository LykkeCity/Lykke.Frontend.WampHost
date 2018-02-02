using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Security;
using Lykke.Job.TradesConverter.Contract;
using Lykke.RabbitMqBroker.Subscriber;
using WampSharp.V2;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Trades
{
    [UsedImplicitly]
    public class TradesSubscriber : ISubscriber
    {
        private readonly ILog _log;
        private readonly IRabbitMqSubscribersFactory _subscribersFactory;
        private readonly string _connectionString;
        private IStopable _subscriber;
        private readonly IWampSubject _subject;
        private readonly IClientResolver _clientResolver;

        public TradesSubscriber(
            [NotNull] ILog log,
            [NotNull] IRabbitMqSubscribersFactory subscribersFactory,
            [NotNull] string connectionString,
            [NotNull] IWampHostedRealm realm,
            [NotNull] IClientResolver clientResolver)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _clientResolver = clientResolver ?? throw new ArgumentNullException(nameof(clientResolver));
            _subscribersFactory = subscribersFactory ?? throw new ArgumentNullException(nameof(subscribersFactory));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

            _subject = realm.Services.GetSubject("trades");
        }

        public void Start()
        {
            _subscriber = _subscribersFactory.Create(
                _connectionString,
                MarketType.Spot,
                "tradelog",
                new MessagePackMessageDeserializer<List<TradeLogItem>>(),
                ProcessTradeAsync);
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        private async Task ProcessTradeAsync(List<TradeLogItem> messages)
        {
            if (!messages.Any())
                return;

            try
            {
                var notificationId = _clientResolver.GetNotificationId(messages[0].UserId);

                if (notificationId == null)
                    return;

                _subject.OnNext(new WampEvent
                {
                    Options = new PublishOptions
                    {
                        Eligible = new[] { long.Parse(notificationId) }
                    },
                    Arguments = new object[] { messages }
                });
            }
            catch (Exception ex)
            {
                _log.WriteWarning(nameof(ProcessTradeAsync), messages, "Failed to process trade", ex);
                throw;
            }

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }
    }
}
