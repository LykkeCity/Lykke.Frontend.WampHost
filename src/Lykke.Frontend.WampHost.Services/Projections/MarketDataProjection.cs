using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Exchange.Api.MarketData.Contract;
using Microsoft.Extensions.Caching.Memory;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Projections
{
    [UsedImplicitly]
    public class MarketDataProjection
    {
        [NotNull] private readonly IWampHostedRealm _realm;
        private readonly IMemoryCache _memoryCache;
        private readonly ISubject<MarketDataChangedEvent> _subject;
        private const string MarketDataTopic = "marketdata";
        private readonly TimeSpan _cacheInterval;
        
        public MarketDataProjection(
            [NotNull] IWampHostedRealm realm,
            TimeSpan cacheInterval,
            IMemoryCache memoryCache)
        {
            _realm = realm;
            _cacheInterval = cacheInterval;
            _memoryCache = memoryCache;
            _subject = _realm.Services.GetSubject<MarketDataChangedEvent>(MarketDataTopic);
        }
        
        public Task Handle(MarketDataChangedEvent evt)
        {
            if (_memoryCache.TryGetValue(evt.AssetPairId, out _))
                return Task.CompletedTask;
            
            _subject.OnNext(evt);
            var assetPairSubject = _realm.Services.GetSubject<MarketDataChangedEvent>($"{MarketDataTopic}.{evt.AssetPairId}");
            assetPairSubject.OnNext(evt);
            _memoryCache.Set(evt.AssetPairId, evt.AssetPairId, _cacheInterval);
            return Task.CompletedTask;
        }
    }
}
