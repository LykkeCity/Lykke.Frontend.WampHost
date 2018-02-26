using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services.TradesAnon;
using Lykke.Job.TradesConverter.Contract;
using Lykke.Service.Assets.Client;
using Lykke.Service.TradesAdapter.Contract;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.TradesAnon
{
    [UsedImplicitly]
    public class TradesAnonManager : ITradesAnonManager
    {
        private readonly IWampHostedRealm _realm;

        private readonly IAssetsServiceWithCache _assetsServiceWithCache;

        public TradesAnonManager(IWampHostedRealm realm, IAssetsServiceWithCache assetsServiceWithCache)
        {
            _realm = realm;
            _assetsServiceWithCache = assetsServiceWithCache;
        }

        public async Task ProcessTrade(Trade tradeLogItem, MarketType market)
        {
            var topic = $"trades.{market.ToString().ToLower()}.{tradeLogItem.AssetPairId.ToLower()}";
            var subject = _realm.Services.GetSubject<Trade>(topic);

            subject.OnNext(tradeLogItem);
        }

        private async Task<string> GetAssetPairId(string asset1, string asset2)
        {
            var assetPairs = await _assetsServiceWithCache.GetAllAssetPairsAsync();
            var assetPair = assetPairs.FirstOrDefault(x =>
                x.BaseAssetId == asset1 && x.QuotingAssetId == asset2 ||
                x.BaseAssetId == asset2 && x.QuotingAssetId == asset1);
            return assetPair?.Id;
        }
    }
}
