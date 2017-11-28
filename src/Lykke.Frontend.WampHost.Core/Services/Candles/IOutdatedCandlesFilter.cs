using Lykke.Job.CandlesProducer.Contract;

namespace Lykke.Frontend.WampHost.Core.Services.Candles
{
    public interface IOutdatedCandlesFilter
    {
        bool ShouldFilterOut(CandleMessage candle);
    }
}
