using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services.TradesAnon;
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

        public TradesAnonManager(
            IWampHostedRealm realm,
            IDistributedCache cache)
        {
            _realm = realm;
            _cache = cache;
        }

        public async Task ProcessTrade(Trade tradeLogItem, MarketType market)
        {
            if (await _cache.GetAsync(tradeLogItem.Id) == null)
            {
                var topic = $"trades.{market.ToString().ToLower()}.{tradeLogItem.AssetPairId.ToLower()}";
                var subject = _realm.Services.GetSubject<Trade>(topic);

                subject.OnNext(tradeLogItem);

                await _cache.SetAsync(
                    tradeLogItem.Id,
                    new byte[]{},
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddDays(1)
                    });
            }
        }
    }
}
