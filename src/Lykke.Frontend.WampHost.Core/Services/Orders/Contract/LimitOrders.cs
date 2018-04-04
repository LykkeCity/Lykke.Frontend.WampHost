using System.Collections.Generic;

namespace Lykke.Frontend.WampHost.Core.Orders.Contract
{
    public class LimitOrders
    {
        public List<LimitOrderWithTrades> Orders { get; set; }
    }
}
