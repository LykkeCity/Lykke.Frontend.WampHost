using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
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

        public CandlesManager(IWampHostedRealm realm)
        {
            _realm = realm;
        }

        public void ProcessCandles(CandlesUpdatedEvent updatedCandles, MarketType market)
        {
            foreach (var candle in updatedCandles.Candles)
            {
                ProcessCandleAsync(candle, market);
            }
        }

        private void ProcessCandleAsync(CandleUpdate candle, MarketType market)
        {
            var topic = $"candle.{market.ToString().ToLower()}.{candle.AssetPairId.ToLower()}.{candle.PriceType.ToString().ToLower()}.{candle.TimeInterval.ToString().ToLower()}";
            var subject = _realm.Services.GetSubject<CandleClientMessage>(topic);

            subject.OnNext(new CandleClientMessage
            {
                AssetPairId = candle.AssetPairId,
                MarketType = market,
                PriceType = candle.PriceType,
                TimeInterval = candle.TimeInterval,
                Timestamp = candle.CandleTimestamp,
                Open = candle.Open,
                Close = candle.Close,
                High = candle.High,
                Low = candle.Low,
                TradingVolume = candle.TradingVolume,
                OppositeTradingVolume = candle.TradingOppositeVolume
            });
        }
    }
}
