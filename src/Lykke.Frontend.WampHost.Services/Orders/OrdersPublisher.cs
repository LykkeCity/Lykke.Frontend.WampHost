using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Orders;
using Lykke.Frontend.WampHost.Core.Orders.Contract;
using Lykke.Frontend.WampHost.Core.Services.Orders;
using Lykke.Frontend.WampHost.Core.Services.Orders.OutgoingMessages;
using Lykke.Frontend.WampHost.Core.Services.Security;
using Lykke.Service.ClientAccount.Client;
using WampSharp.V2;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Orders
{
    [UsedImplicitly]
    public class OrdersPublisher : IOrdersPublisher
    {
        private readonly IClientAccountClient _clientAccountClient;
        private readonly IOrdersConverter _ordersConverter;
        private readonly ISessionCache _sessionCache;
        private readonly IWampSubject _subject;

        private const string Topic = "orders.spot";

        public OrdersPublisher(
            [NotNull] IClientAccountClient clientAccountClient,
            [NotNull] IOrdersConverter ordersConverter,
            [NotNull] ISessionCache sessionCache,
            [NotNull] IWampHostedRealm realm)
        {
            _clientAccountClient = clientAccountClient ?? throw new ArgumentNullException(nameof(clientAccountClient));
            _ordersConverter = ordersConverter ?? throw new ArgumentNullException(nameof(ordersConverter));
            _sessionCache = sessionCache ?? throw new ArgumentNullException(nameof(sessionCache));
            _subject = realm?.Services.GetSubject(Topic) ?? throw new ArgumentNullException(nameof(realm));
        }

        public async Task Publish(MarketOrderWithTrades marketOrderWithTrades)
        {
            if (marketOrderWithTrades == null)
                return;

            var clientId = await _clientAccountClient.GetClientByWalletAsync(marketOrderWithTrades.Order.ClientId);
            var order = await _ordersConverter.ConvertAsync(marketOrderWithTrades.Order);

            PublishOrdersToClient(clientId, new[] { order });
        }

        public async Task Publish(LimitOrders limitOrders)
        {
            if (limitOrders?.Orders == null || !limitOrders.Orders.Any())
                return;

            var idsMappings = new Dictionary<string, string>();
            var ordersByClients = new Dictionary<string, List<LimitOrderWithTrades>>();

            foreach (var order in limitOrders.Orders)
            {
                var walletId = order.Order.ClientId;
                
                if(!idsMappings.ContainsKey(walletId))
                    idsMappings[walletId] = await _clientAccountClient.GetClientByWalletAsync(walletId);

                var clientId = idsMappings[walletId];

                if(!ordersByClients.ContainsKey(clientId))
                    ordersByClients[clientId] = new List<LimitOrderWithTrades>();

                ordersByClients[clientId].Add(order);
            }

            foreach (var clientId in ordersByClients.Keys)
            {
                var orders = await ordersByClients[clientId].SelectAsync(async x =>
                    await _ordersConverter.ConvertAsync(x.Order, x.Trades.Any()));

                PublishOrdersToClient(clientId, orders.ToArray());
            }
        }

        private void PublishOrdersToClient(string clientId, Order[] orders)
        {
            var sessionIds = _sessionCache.GetSessionIds(clientId);

            if (sessionIds.Length == 0)
                return;

            _subject.OnNext(new WampEvent
            {
                Options = new PublishOptions { Eligible = sessionIds },
                Arguments = new [] { orders }
            });
        }
    }
}
