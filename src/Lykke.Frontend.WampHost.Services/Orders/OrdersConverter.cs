using System;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Orders;
using Lykke.Frontend.WampHost.Core.Orders.Contract;
using Lykke.Frontend.WampHost.Core.Services.Orders.OutgoingMessages;

namespace Lykke.Frontend.WampHost.Services.Orders
{
    [UsedImplicitly]
    public class OrdersConverter : IOrdersConverter
    {
        private readonly ILog _log;

        public OrdersConverter(ILog log)
        {
            _log = log;
        }

        public Order Convert(MarketOrder order)
        {
            var status = GetOrderStatus(order.Status);
            
            return new Order
            {
                Id = order.ExternalId,
                Status = status,
                RejectReason = status == OrderStatus.Rejected ? order.Status : null,
                AssetPairId = order.AssetPairId,
                Price = order.Price,
                
                Volume = Math.Abs(order.Volume),
                OrderAction = order.Volume > 0 ? OrderAction.Buy : OrderAction.Sell,
                RemainingVolume = order.MatchedAt != null ? 0 : Math.Abs(order.Volume),
                Straight = order.Straight,
                Type = OrderType.Market,
                CreateDateTime = order.CreatedAt
            };
        }

        public Order Convert(LimitOrder order, bool hasTrades)
        {
            var status = GetOrderStatus(order.Status);

            //ME bug workaround
            if (status == OrderStatus.Processing  && !hasTrades)
                status = OrderStatus.InOrderBook;

            return new Order
            {
                Id = order.ExternalId,
                Status = status,
                RejectReason = status == OrderStatus.Rejected 
                    ? order.Status 
                    : null,
                AssetPairId = order.AssetPairId,
                Price = status == OrderStatus.Pending 
                    ? (double?) null 
                    : order.Price,
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
            try
            {
                var parsed = MeOrderStatus.TryParse(status, true, out MeOrderStatus meStatusCode);

                if (parsed)
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

                throw new ArgumentException(
                    $"Status {status} did not match any of the expected ME orders' status codes");
            }
            catch (Exception e)
            {
                _log.WriteError(nameof(GetOrderStatus), status, e);
                return OrderStatus.Rejected;
            }
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
