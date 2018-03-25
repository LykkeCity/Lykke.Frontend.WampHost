using System.Threading.Tasks;
using Lykke.Frontend.WampHost.Core.Orders.Contract;
using Lykke.Frontend.WampHost.Core.Services.Orders.OutgoingMessages;

namespace Lykke.Frontend.WampHost.Core.Orders
{
    public interface IOrdersConverter
    {
        Task<Order> Convert(MarketOrder order);
        Task<Order> Convert(LimitOrder order);
    }
}
