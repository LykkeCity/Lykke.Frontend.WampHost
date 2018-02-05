using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Orderbook;
using Lykke.Frontend.WampHost.Services.Extensions;
using Lykke.RabbitMqBroker.Subscriber;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Orderbooks.Spot
{
    [UsedImplicitly]
    public class SpotOrderbookSubscriber : ISubscriber
    {
        private readonly ILog _log;
        private readonly IRabbitMqSubscribeHelper _rabbitMqSubscribeHelper;
        private readonly string _connectionString;
        private readonly IWampHostedRealm _realm;

        public SpotOrderbookSubscriber(
            IRabbitMqSubscribeHelper rabbitMqSubscribeHelper,
            string connectionString,
            ILog log, IWampHostedRealm realm)
        {
            _log = log;
            _realm = realm;
            _rabbitMqSubscribeHelper = rabbitMqSubscribeHelper;
            _connectionString = connectionString;
        }

        public void Start()
        {
            _rabbitMqSubscribeHelper.Subscribe(
                _connectionString,
                MarketType.Spot,
                "orderbook",
                new JsonMessageDeserializer<OrderbookMessage>(),
                ProcessOrderbookAsync);
        }

        private async Task ProcessOrderbookAsync(OrderbookMessage orderbookMessage)
        {
            try
            {
                var topic = $"orderbook.spot.{orderbookMessage.AssetPair.ToLower()}.{(orderbookMessage.IsBuy ? "buy" : "sell")}";
                var subject = _realm.Services.GetSubject<OrderbookModel>(topic);

                subject.OnNext(orderbookMessage.ToModel());
            }
            catch (Exception)
            {
                _log.WriteWarning(nameof(ProcessOrderbookAsync), orderbookMessage, "Failed to process orderbook");
                throw;
            }

            await Task.CompletedTask;
        }
    }
}
