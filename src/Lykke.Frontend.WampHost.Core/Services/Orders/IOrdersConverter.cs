using Lykke.Frontend.WampHost.Core.Orders.Contract;
using Lykke.Frontend.WampHost.Core.Services.Orders.OutgoingMessages;

namespace Lykke.Frontend.WampHost.Core.Orders
{
    public interface IOrdersConverter
    {
        Order Convert(MarketOrder order);
        Order Convert(LimitOrder order, bool hasTrades);
    }
}
