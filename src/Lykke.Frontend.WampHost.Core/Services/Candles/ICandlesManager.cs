using System.Threading.Tasks;
using Lykke.Frontend.WampHost.Core.Domain.Candles;
using Lykke.Job.CandlesProducer.Contract;

namespace Lykke.Frontend.WampHost.Core.Services.Candles
{
    public interface ICandlesManager
    {
        void ProcessCandle(CandleMessage candle, MarketType marketType);
    }
}
