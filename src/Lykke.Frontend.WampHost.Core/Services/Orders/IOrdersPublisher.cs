using System.Threading.Tasks;
using Lykke.Frontend.WampHost.Core.Orders.Contract;

namespace Lykke.Frontend.WampHost.Core.Services.Orders
{
    public interface IOrdersPublisher
    {
        Task Publish(MarketOrderWithTrades marketOrderWithTrades);
        Task Publish(LimitOrders limitOrders);
    }
}
