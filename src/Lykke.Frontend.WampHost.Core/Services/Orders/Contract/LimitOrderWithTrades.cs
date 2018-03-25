using System.Collections.Generic;

namespace Lykke.Frontend.WampHost.Core.Orders.Contract
{
    public class LimitOrderWithTrades
    {
        public LimitOrder Order { get; set; }

        public List<LimitTradeInfo> Trades { get; set; }
    }
}
