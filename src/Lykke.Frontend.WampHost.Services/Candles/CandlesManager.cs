using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Lykke.Domain.Prices;
using Lykke.Frontend.WampHost.Core.Domain.Candles;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Candles
{
    public interface ICandlesManager
    {
        Task ProcessCandleAsync(CandleMessage candle, MarketType marketType);
    }

    public class CandlesManager : ICandlesManager
    {
        private readonly IWampHostedRealm _realm;
        
        public CandlesManager(IWampHostedRealm realm)
        {
            _realm = realm;            
        }

        public Task ProcessCandleAsync(CandleMessage candle, MarketType marketType)
        {
            var topic = $"candle.{marketType.ToString().ToLower()}.{candle.AssetPairId.ToLower()}.{candle.PriceType.ToString().ToLower()}.{candle.TimeInterval.ToString().ToLower()}";

            var subject = _realm.Services.GetSubject<CandleClientMessage>(topic);

            subject.OnNext(new CandleClientMessage
            {
                AssetPairId = candle.AssetPairId,
                PriceType = candle.PriceType,
                TimeInterval = candle.TimeInterval,
                Timestamp = candle.Timestamp,
                Open = candle.Open,
                Close = candle.Close,
                High = candle.High,
                Low = candle.Low
            });

            return Task.FromResult(0);
        }        
    }
}