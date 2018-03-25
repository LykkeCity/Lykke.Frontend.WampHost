using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Orders;
using Lykke.Frontend.WampHost.Core.Orders.Contract;
using Lykke.Frontend.WampHost.Core.Services.Orders.OutgoingMessages;

namespace Lykke.Frontend.WampHost.Services.Orders
{
    [UsedImplicitly]
    public class OrdersConverter : IOrdersConverter
    {
        public Task<Order> Convert(MarketOrder order)
        {
            return Task.FromResult(new Order
            {
                Id = order.ExternalId,
                Status = order.Status
            });
        }

        public Task<Order> Convert(LimitOrder order)
        {
            return Task.FromResult(new Order
            {
                Id = order.ExternalId,
                Status = order.Status
            });
        }
    }
}
