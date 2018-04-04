using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Orders.Contract;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Orders;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.TradesAdapter.Contract;

namespace Lykke.Frontend.WampHost.Services.Orders
{
    [UsedImplicitly]
    public class LimitOrdersSubscriber : ISubscriber
    {
        private readonly ILog _log;
        private readonly IOrdersPublisher _ordersPublisher;
        private readonly IRabbitMqSubscribeHelper _rabbitMqSubscribeHelper;
        private readonly string _connectionString;
        private readonly MarketType _marketType;

        public LimitOrdersSubscriber(
            [NotNull] IRabbitMqSubscribeHelper rabbitMqSubscribeHelper,
            [NotNull] IOrdersPublisher ordersPublisher,
            [NotNull] string connectionString,
            [NotNull] MarketType marketType,
            [NotNull] ILog log)
        {
            _rabbitMqSubscribeHelper = rabbitMqSubscribeHelper ?? throw new ArgumentNullException(nameof(rabbitMqSubscribeHelper));
            _ordersPublisher = ordersPublisher ?? throw new ArgumentNullException(nameof(ordersPublisher));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _marketType = marketType;
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }
        
        public void Start()
        {
            _rabbitMqSubscribeHelper.Subscribe(
                connectionString: _connectionString,
                market: _marketType,
                source: "limitorders.clients",
                deserializer: new JsonMessageDeserializer<LimitOrders>(),
                handler: ProcessMessageAsync);
        }

        private async Task ProcessMessageAsync(LimitOrders orders)
        {
            _ordersPublisher.Publish(orders);
        }
    }
}
