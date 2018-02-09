using System.Linq;
using System.Threading.Tasks;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services.TradesAnon;
using Lykke.Frontend.WampHost.Services.TradesAnon.Contract;
using Lykke.Job.TradesConverter.Contract;
using Lykke.Service.Assets.Client;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.TradesAnon
{
    public class TradesAnonManager : ITradesAnonManager
    {
        private readonly IWampHostedRealm _realm;

        private readonly IAssetsServiceWithCache _assetsServiceWithCache;

        public TradesAnonManager(IWampHostedRealm realm, IAssetsServiceWithCache assetsServiceWithCache)
        {
            _realm = realm;
            _assetsServiceWithCache = assetsServiceWithCache;
        }

        public async Task ProcessTrade(TradeLogItem tradeLogItem, MarketType market)
        {
            var pairId = await GetAssetPairId(tradeLogItem.Asset, tradeLogItem.OppositeAsset);

            if (pairId == null)
                return;

            var topic = $"trades.{market.ToString().ToLower()}.{pairId.ToLower()}";
            var subject = _realm.Services.GetSubject<TradeAnonClientMessage>(topic);

            subject.OnNext(tradeLogItem.ToTradeAnonClientMessage());
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
