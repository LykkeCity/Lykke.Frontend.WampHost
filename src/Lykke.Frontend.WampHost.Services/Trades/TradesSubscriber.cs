using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IRabbitMqSubscribeHelper _rabbitMqSubscribeHelper;
        private readonly string _connectionString;
        private readonly IWampSubject _subject;
        private readonly ISessionCache _sessionCache;

        public TradesSubscriber(
            [NotNull] ILog log,
            [NotNull] IRabbitMqSubscribeHelper rabbitMqSubscribeHelper,
            [NotNull] string connectionString,
            [NotNull] IWampHostedRealm realm,
            [NotNull] ISessionCache sessionCache)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _sessionCache = sessionCache ?? throw new ArgumentNullException(nameof(sessionCache));
            _rabbitMqSubscribeHelper = rabbitMqSubscribeHelper ?? throw new ArgumentNullException(nameof(rabbitMqSubscribeHelper));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

            _subject = realm.Services.GetSubject("trades");
        }

        public void Start()
        {
            _rabbitMqSubscribeHelper.Subscribe(
                _connectionString,
                MarketType.Spot,
                "tradelog",
                new MessagePackMessageDeserializer<List<TradeLogItem>>(),
                ProcessTradeAsync);
        }

        private async Task ProcessTradeAsync(List<TradeLogItem> messages)
        {
            if (!messages.Any())
                return;

            try
            {
                var sessionIds = _sessionCache.GetSessionIds(messages[0].UserId);
                if (sessionIds.Length == 0)
                    return;

                _subject.OnNext(new WampEvent
                {
                    Options = new PublishOptions
                    {
                        Eligible = sessionIds
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
    }
}
