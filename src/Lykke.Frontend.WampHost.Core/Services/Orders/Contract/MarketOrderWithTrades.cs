using System.Collections.Generic;

namespace Lykke.Frontend.WampHost.Core.Orders.Contract
{
    public class MarketOrderWithTrades
    {
        public MarketOrder Order { get; set; }

        public List<TradeInfo> Trades { get; set; }
    }
}
