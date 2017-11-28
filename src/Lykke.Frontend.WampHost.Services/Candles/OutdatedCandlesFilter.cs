using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Services.Candles;
using Lykke.Job.CandlesProducer.Contract;

namespace Lykke.Frontend.WampHost.Services.Candles
{
    [UsedImplicitly]
    public class OutdatedCandlesFilter : IOutdatedCandlesFilter
    {
        public bool ShouldFilterOut(CandleMessage candle)
        {
            throw new System.NotImplementedException();
        }
    }
}
