using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Domain.Candles;
using Lykke.Frontend.WampHost.Core.Services.Candles;
using Lykke.Frontend.WampHost.Services.Candles.Contract;
using Lykke.Job.CandlesProducer.Contract;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Candles
{
    [UsedImplicitly]
    public class CandlesManager : ICandlesManager
    {
        private readonly IWampHostedRealm _realm;
        private readonly IOutdatedCandlesFilter _outdatedCandlesFilter;

        public CandlesManager(IWampHostedRealm realm, IOutdatedCandlesFilter outdatedCandlesFilter)
        {
            _realm = realm;
            _outdatedCandlesFilter = outdatedCandlesFilter;
        }

        public void ProcessCandle(CandleMessage candle, MarketType marketType)
        {
            if (_outdatedCandlesFilter.ShouldFilterOut(candle))
            {
                return;
            }

            var topic = $"candle.{marketType.ToString().ToLower()}.{candle.AssetPairId.ToLower()}.{candle.PriceType.ToString().ToLower()}.{candle.TimeInterval.ToString().ToLower()}";
            var subject = _realm.Services.GetSubject<CandleClientMessage>(topic);

            subject.OnNext(new CandleClientMessage
            {
                AssetPairId = candle.AssetPairId,
                MarketType = marketType,
                PriceType = candle.PriceType,
                TimeInterval = candle.TimeInterval,
                Timestamp = candle.Timestamp,
                Open = candle.Open,
                Close = candle.Close,
                High = candle.High,
                Low = candle.Low,
                TradingVolume = candle.TradingVolume
            });
        }        
    }
}
