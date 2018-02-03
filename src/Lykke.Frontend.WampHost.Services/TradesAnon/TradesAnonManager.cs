using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services.TradesAnon;
using Lykke.Frontend.WampHost.Services.TradesAnon.Contract;
using Lykke.Job.TradesConverter.Contract;
using Lykke.Service.Assets.Client.Models;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.TradesAnon
{
    public class TradesAnonManager : ITradesAnonManager
    {
        private readonly IWampHostedRealm _realm;
        private readonly CachedDataDictionary<string, AssetPair> _assetPairsCache;
        
        public TradesAnonManager(IWampHostedRealm realm, CachedDataDictionary<string, AssetPair> assetPairsCache)
        {
            _realm = realm;
            _assetPairsCache = assetPairsCache;
        }
        
        public async Task ProcessTrade(TradeLogItem tradeLogItem, MarketType market)
        {
            var pairId = await GetAssetPairId(tradeLogItem.Asset, tradeLogItem.OppositeAsset);

            if (string.IsNullOrEmpty(pairId))
                return;
            
            var topic = $"trades.{market.ToString().ToLower()}.{pairId}";
            var subject = _realm.Services.GetSubject<TradeAnonClientMessage>(topic);
            
            subject.OnNext(tradeLogItem.ToTradeAnonClientMessage());
        }

        private async Task<string> GetAssetPairId(string asset1, string asset2)
        {
            var assetPairs = await _assetPairsCache.Values();
            var assetPair = assetPairs.FirstOrDefault(x =>
                (x.BaseAssetId == asset1 && x.QuotingAssetId == asset2) ||
                (x.BaseAssetId == asset2 && x.QuotingAssetId == asset1));
            return assetPair?.Id ?? string.Empty;
        }
    }
}
