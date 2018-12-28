using System.Threading.Tasks;
using Lykke.Frontend.WampHost.Core.Orders.Contract;
using Lykke.Frontend.WampHost.Core.Services.Orders.OutgoingMessages;

namespace Lykke.Frontend.WampHost.Core.Orders
{
    public interface IOrdersConverter
    {
        Task<Order> ConvertAsync(MarketOrder order);
        Task<Order> ConvertAsync(LimitOrder order, bool hasTrades);
    }
}
