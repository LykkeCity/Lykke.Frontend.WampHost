using System.Collections.Generic;
using Lykke.Job.TradesConverter.Contract;

namespace Lykke.Frontend.WampHost.Core.Services.Trades
{
    public interface ITradesManager
    {
        void ProcessTrade(List<TradeLogItem> messages);
    }
}
