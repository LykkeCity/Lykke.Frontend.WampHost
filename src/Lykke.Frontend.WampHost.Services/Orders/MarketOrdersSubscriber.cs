using System;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Orders.Contract;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Orders;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Frontend.WampHost.Services.Orders
{
    [UsedImplicitly]
    public class MarketOrdersSubscriber : ISubscriber
    {
        private readonly ILog _log;
        private readonly IRabbitMqSubscribeHelper _rabbitMqSubscribeHelper;
        private readonly string _connectionString;
        private readonly string _exchangeName;
        private readonly MarketType _marketType;
        private readonly IOrdersPublisher _ordersPublisher;

        public MarketOrdersSubscriber(
            [NotNull] IRabbitMqSubscribeHelper rabbitMqSubscribeHelper,
            [NotNull] IOrdersPublisher ordersPublisher,
            [NotNull] string connectionString,
            [NotNull] string exchangeName,
            [NotNull] MarketType marketType,
            [NotNull] ILog log)
        {
            _rabbitMqSubscribeHelper = rabbitMqSubscribeHelper ?? throw new ArgumentNullException(nameof(rabbitMqSubscribeHelper));
            _ordersPublisher = ordersPublisher ?? throw new ArgumentNullException(nameof(ordersPublisher));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _marketType = marketType;
            _exchangeName = exchangeName ?? throw new ArgumentNullException(nameof(exchangeName));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }
        
        public void Start()
        {
            _rabbitMqSubscribeHelper.Subscribe(
                connectionString: _connectionString,
                market: _marketType,
                source: _exchangeName,
                deserializer: new JsonMessageDeserializer<MarketOrderWithTrades>(),
                handler: ProcessMessageAsync);
        }

        private async Task ProcessMessageAsync(MarketOrderWithTrades orders)
        {
            _ordersPublisher.Publish(orders);
        }
    }
}
