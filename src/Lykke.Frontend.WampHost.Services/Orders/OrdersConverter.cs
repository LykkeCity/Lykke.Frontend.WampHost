using System;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Orders;
using Lykke.Frontend.WampHost.Core.Orders.Contract;
using Lykke.Frontend.WampHost.Core.Services.Clients;
using Lykke.Frontend.WampHost.Core.Services.Orders.OutgoingMessages;

namespace Lykke.Frontend.WampHost.Services.Orders
{
    [UsedImplicitly]
    public class OrdersConverter : IOrdersConverter
    {
        private readonly ILog _log;
        private readonly IClientToWalletMapper _clientToWalletMapper;

        public OrdersConverter(ILog log, [NotNull] IClientToWalletMapper clientToWalletMapper)
        {
            _log = log;
            _clientToWalletMapper = clientToWalletMapper ?? throw new ArgumentNullException(nameof(clientToWalletMapper));
        }

        public async Task<Order> ConvertAsync(MarketOrder order)
        {
            var status = GetOrderStatus(order.Status);
            var (_, walletId) = await _clientToWalletMapper.GetClientIdAndWalletIdAsync(order.ClientId);

            return new Order
            {
                Id = order.ExternalId,
                Status = status,
                RejectReason = status == OrderStatus.Rejected ? order.Status : null,
                AssetPairId = order.AssetPairId,
                Price = order.Price,
                WalletId = walletId,
                Volume = Math.Abs(order.Volume),
                OrderAction = order.Volume > 0 ? OrderAction.Buy : OrderAction.Sell,
                RemainingVolume = order.MatchedAt != null ? 0 : Math.Abs(order.Volume),
                Straight = order.Straight,
                Type = OrderType.Market,
                CreateDateTime = order.CreatedAt
            };
        }

        public async Task<Order> ConvertAsync(LimitOrder order, bool hasTrades)
        {
            var status = GetOrderStatus(order.Status);

            //ME bug workaround
            if (status == OrderStatus.Processing && !hasTrades)
                status = OrderStatus.InOrderBook;

            var (_, walletId) = await _clientToWalletMapper.GetClientIdAndWalletIdAsync(order.ClientId);

            return new Order
            {
                Id = order.ExternalId,
                Status = status,
                RejectReason = status == OrderStatus.Rejected
                    ? order.Status
                    : null,
                AssetPairId = order.AssetPairId,
                Price = status == OrderStatus.Pending
                    ? (double?)null
                    : order.Price,
                WalletId = walletId,
                LowerLimitPrice = order.LowerLimitPrice,
                LowerPrice = order.LowerPrice,
                UpperLimitPrice = order.UpperLimitPrice,
                UpperPrice = order.UpperPrice,
                Volume = Math.Abs(order.Volume),
                OrderAction = order.Volume > 0
                    ? OrderAction.Buy
                    : OrderAction.Sell,
                RemainingVolume = Math.Abs(order.RemainingVolume),
                Straight = order.Straight,
                Type = GetLimitOrderType(order),
                CreateDateTime = order.CreatedAt
            };
        }

        private OrderStatus GetOrderStatus(string status)
        {
            if (MeOrderStatus.TryParse(status, true, out MeOrderStatus meStatusCode))
            {
                switch (meStatusCode)
                {
                    case MeOrderStatus.InOrderBook:
                        return OrderStatus.InOrderBook;
                    case MeOrderStatus.Cancelled:
                        return OrderStatus.Cancelled;
                    case MeOrderStatus.Matched:
                        return OrderStatus.Matched;
                    case MeOrderStatus.Processing:
                        return OrderStatus.Processing;
                    case MeOrderStatus.Pending:
                        return OrderStatus.Pending;
                    default:
                        return OrderStatus.Rejected;
                }
            }

            _log.WriteWarning(nameof(GetOrderStatus), status, $"Status {status} did not match any of the expected ME orders' status codes");
            return OrderStatus.Rejected;
        }

        private OrderType GetLimitOrderType(LimitOrder order)
        {
            return order.LowerPrice.HasValue || order.LowerLimitPrice.HasValue ||
                   order.UpperPrice.HasValue || order.UpperLimitPrice.HasValue
                ? OrderType.StopLimit
                : OrderType.Limit;
        }
    }
}
