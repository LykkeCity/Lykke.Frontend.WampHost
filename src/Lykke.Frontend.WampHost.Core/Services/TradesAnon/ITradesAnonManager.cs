using System.Threading.Tasks;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Job.TradesConverter.Contract;

namespace Lykke.Frontend.WampHost.Core.Services.TradesAnon
{
    public interface ITradesAnonManager
    {
        Task ProcessTrade(TradeLogItem tradeLogItem, MarketType market);
    }
}
