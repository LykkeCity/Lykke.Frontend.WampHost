using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services.TradesAnon;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Job.TradesConverter.Contract;
using Lykke.Service.Assets.Client;
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
        private readonly RedisSettings _settings;

        public TradesAnonManager(
            IWampHostedRealm realm,
            IDistributedCache cache,
            RedisSettings settings)
        {
            _realm = realm;
            _cache = cache;
            _settings = settings;
        }

        public async Task ProcessTrade(Trade tradeLogItem, MarketType market)
        {
            var redisKey = _settings.GetKeyForTradeAnonId(tradeLogItem.Id);
            
            if (await _cache.GetAsync(redisKey) == null)
            {
                var topic = $"trades.{market.ToString().ToLower()}.{tradeLogItem.AssetPairId.ToLower()}";
                var subject = _realm.Services.GetSubject<Trade>(topic);

                subject.OnNext(tradeLogItem);

                await _cache.SetAsync(
                    redisKey,
                    Encoding.UTF8.GetBytes("data"),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddDays(1)
                    });
            }
        }
    }
}
