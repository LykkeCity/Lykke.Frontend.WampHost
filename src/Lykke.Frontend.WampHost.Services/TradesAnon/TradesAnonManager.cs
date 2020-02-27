using System;
using System.Text;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services.TradesAnon;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Service.TradesAdapter.Contract;
using Microsoft.Extensions.Caching.Distributed;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.TradesAnon
{
    [UsedImplicitly]
    public class TradesAnonManager : ITradesAnonManager
    {
        private readonly IWampHostedRealm _realm;
        private readonly IDistributedCache _cache;
        private readonly CacheSettings _settings;
        private readonly byte[] _redisValue;

        public TradesAnonManager(
            IWampHostedRealm realm,
            IDistributedCache cache,
            CacheSettings settings)
        {
            _realm = realm;
            _cache = cache;
            _settings = settings;
            _redisValue = Encoding.UTF8.GetBytes("data");
        }

        public async Task ProcessTrade(Trade tradeLogItem, MarketType market)
        {
            var redisKey = _settings.GetKeyForTradeAnonId(tradeLogItem.Id);

            if (await _cache.GetAsync(redisKey) == null)
            {
                var topic = $"trades.{market.ToString().ToLower()}.{tradeLogItem.AssetPairId.ToLower()}";
                var subject = _realm.Services.GetSubject<Trade>(topic);

                Console.WriteLine($"Send trade event: {tradeLogItem.ToJson()}");
                subject.OnNext(tradeLogItem);

                await _cache.SetAsync(
                    redisKey,
                    _redisValue,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddDays(1)
                    });
            }
            else
            {
                Console.WriteLine($"Skip sending trade event: {tradeLogItem.ToJson()}");
            }
        }
    }
}
