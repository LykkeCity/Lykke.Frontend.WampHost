using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Lykke.Domain.Prices;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Candles
{
    public interface ICandlesManager
    {
        Task ProcessCandleAsync(CandleMessage candle, string marketType);
    }

    public class CandlesManager : ICandlesManager
    {
        private readonly IWampHostedRealm _realm;
        
        public CandlesManager(IWampHostedRealm realm)
        {
            _realm = realm;            
        }

        public Task ProcessCandleAsync(CandleMessage candle, string marketType)
        {
            var topic = $"candle.{marketType}.{candle.AssetPairId.ToLower()}.{candle.PriceType.ToString().ToLower()}.{candle.TimeInterval.ToString().ToLower()}";

            var subject = _realm.Services.GetSubject<CandleMessage>(topic);
            subject.OnNext(candle);

            return Task.FromResult(0);
        }        
    }
}