using System;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Orders;
using Lykke.Frontend.WampHost.Core.Orders.Contract;
using Lykke.Frontend.WampHost.Core.Services.Orders.OutgoingMessages;

namespace Lykke.Frontend.WampHost.Services.Orders
{
    [UsedImplicitly]
    public class OrdersConverter : IOrdersConverter
    {
        public Order Convert(MarketOrder order)
        {
            Console.WriteLine(order.ToJson());
            return new Order
            {
                Id = order.ExternalId,
                Status = Order.GetOrderStatus(order.Status),
                RejectReason = Order.GetOrderRejectReason(order.Status),
                AssetPairId = order.AssetPairId,
                Price = order.Price,
                Volume = Math.Abs(order.Volume),
                Action = order.Straight ? OrderAction.Buy : OrderAction.Sell,
                RemainingVolume = order.MatchedAt != null ? 0 : Math.Abs(order.Volume),
                Straight = order.Straight,
                Type = OrderType.Market,
                CreatedAt = order.CreatedAt
            };
        }

        public Order Convert(LimitOrder order, bool hasTrades)
        {
            string status = order.Status;

            //ME bug workaround
            if (order.Status == "Processing" && !hasTrades)
                status = "InOrderBook";
            
            return new Order
            {
                Id = order.ExternalId,
                Status = Order.GetOrderStatus(status),
                RejectReason = Order.GetOrderRejectReason(status),
                AssetPairId = order.AssetPairId,
                Price = order.Price,
                Volume = Math.Abs(order.Volume),
                Action = order.Volume > 0 ? OrderAction.Buy : OrderAction.Sell,
                RemainingVolume = Math.Abs(order.RemainingVolume),
                Straight = order.Straight,
                Type = OrderType.Limit,
                CreatedAt = order.CreatedAt
            };
        }
    }
}
