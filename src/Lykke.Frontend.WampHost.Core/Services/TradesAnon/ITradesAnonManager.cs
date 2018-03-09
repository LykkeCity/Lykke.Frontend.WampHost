using System.Threading.Tasks;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Job.TradesConverter.Contract;
using Lykke.Service.TradesAdapter.Contract;

namespace Lykke.Frontend.WampHost.Core.Services.TradesAnon
{
    public interface ITradesAnonManager
    {
        Task ProcessTrade(Trade tradeLogItem, MarketType market);
    }
}
