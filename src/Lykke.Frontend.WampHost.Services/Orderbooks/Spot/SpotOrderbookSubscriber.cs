using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Orderbook;
using Lykke.Frontend.WampHost.Services.Extensions;
using Lykke.Frontend.WampHost.Services.Quotes.Spot;
using Lykke.RabbitMqBroker.Subscriber;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Orderbooks.Spot
{
    [UsedImplicitly]
    public class SpotOrderbookSubscriber : ISubscriber
    {
        private readonly ILog _log;
        private readonly IRabbitMqSubscribersFactory _subscribersFactory;
        private readonly string _connectionString;
        private readonly IWampHostedRealm _realm;

        private IStopable _subscriber;

        public SpotOrderbookSubscriber(
            IRabbitMqSubscribersFactory subscribersFactory,
            string connectionString,
            ILog log, IWampHostedRealm realm)
        {
            _log = log;
            _realm = realm;
            _subscribersFactory = subscribersFactory;
            _connectionString = connectionString;
        }

        public void Start()
        {
            _subscriber = _subscribersFactory.Create(
                _connectionString,
                MarketType.Spot,
                "orderbook",
                new JsonMessageDeserializer<OrderbookMessage>(),
                ProcessOrderbookAsync);
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        private async Task ProcessOrderbookAsync(OrderbookMessage orderbookMessage)
        {
            try
            {
                var topic = $"orderbook.{orderbookMessage.AssetPair.ToLower()}.{(orderbookMessage.IsBuy ? "buy" : "sell")}";
                var subject = _realm.Services.GetSubject<OrderbookModel>(topic);

                subject.OnNext(orderbookMessage.ToModel());
            }
            catch (Exception)
            {
                await _log.WriteWarningAsync(nameof(SpotQuotesSubscriber), nameof(ProcessOrderbookAsync), orderbookMessage?.ToJson(), "Failed to process orderbook");
                throw;
            }
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }
    }
}
