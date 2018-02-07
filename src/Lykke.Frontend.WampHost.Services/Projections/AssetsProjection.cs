using System.Reactive.Subjects;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Services.Assets.Contracts;
using Lykke.Frontend.WampHost.Services.Assets.IncomeMessages;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Projections
{
    public class AssetsProjection
    {
        private readonly ILog _log;
        private readonly ISubject<AssetUpdateMessage> _subjectAssets;
        private readonly ISubject<AssetPairUpdateMessage> _subjectAssetPairs;
        private const string AssetsTopicUri = "assets";
        private const string AssetPairsTopicUri = "assetpairs";

        public AssetsProjection(
            [NotNull] ILog log,
            [NotNull] IWampHostedRealm realm)
        {
            _log = log.CreateComponentScope(nameof(AssetsProjection));
            _subjectAssets = realm.Services.GetSubject<AssetUpdateMessage>(AssetsTopicUri);
            _subjectAssetPairs = realm.Services.GetSubject<AssetPairUpdateMessage>(AssetPairsTopicUri);
        }

        public async Task Handle(AssetCreatedEvent evt)
        {
            _log.WriteInfo(nameof(AssetCreatedEvent), evt, "");

            _subjectAssets.OnNext(new AssetUpdateMessage
            {
                Id = evt.Asset.Id,
                Name = evt.Asset.Name,
                Accuracy = evt.Asset.Accuracy,
                BankCardsDepositEnabled = evt.Asset.BankCardsDepositEnabled,
                BlockchainDepositEnabled = evt.Asset.BlockchainDepositEnabled,
                CategoryId = evt.Asset.CategoryId,
                DisplayId = evt.Asset.DisplayId,
                HideDeposit = evt.Asset.HideDeposit,
                HideWithdraw = evt.Asset.HideWithdraw,
                IconUrl = evt.Asset.IconUrl,
                IsBase = evt.Asset.IsBase,
                KycNeeded = evt.Asset.KycNeeded,
                SwiftDepositEnabled = evt.Asset.SwiftDepositEnabled,
                Symbol = evt.Asset.Symbol
            });
        }

        public async Task Handle(AssetUpdatedEvent evt)
        {
            _log.WriteInfo(nameof(AssetUpdatedEvent), evt, "");

            _subjectAssets.OnNext(new AssetUpdateMessage
            {
                Id = evt.Asset.Id,
                Name = evt.Asset.Name,
                Accuracy = evt.Asset.Accuracy,
                BankCardsDepositEnabled = evt.Asset.BankCardsDepositEnabled,
                BlockchainDepositEnabled = evt.Asset.BlockchainDepositEnabled,
                CategoryId = evt.Asset.CategoryId,
                DisplayId = evt.Asset.DisplayId,
                HideDeposit = evt.Asset.HideDeposit,
                HideWithdraw = evt.Asset.HideWithdraw,
                IconUrl = evt.Asset.IconUrl,
                IsBase = evt.Asset.IsBase,
                KycNeeded = evt.Asset.KycNeeded,
                SwiftDepositEnabled = evt.Asset.SwiftDepositEnabled,
                Symbol = evt.Asset.Symbol
            });
        }

        public async Task Handle(AssetPairCreatedEvent evt)
        {
            _log.WriteInfo(nameof(AssetPairCreatedEvent), evt, "");

            _subjectAssetPairs.OnNext(new AssetPairUpdateMessage
            {
                Id = evt.AssetPair.Id,
                Accuracy = evt.AssetPair.Accuracy,
                Name = evt.AssetPair.Name,
                BaseAssetId = evt.AssetPair.BaseAssetId,
                InvertedAccuracy = evt.AssetPair.InvertedAccuracy,
                IsDisabled = evt.AssetPair.IsDisabled,
                QuotingAssetId = evt.AssetPair.QuotingAssetId,
                Source = evt.AssetPair.Source,
                Source2 = evt.AssetPair.Source2
            });
        }

        public async Task Handle(AssetPairUpdatedEvent evt)
        {
            _log.WriteInfo(nameof(AssetPairUpdatedEvent), evt, "");

            _subjectAssetPairs.OnNext(new AssetPairUpdateMessage
            {
                Id = evt.AssetPair.Id,
                Accuracy = evt.AssetPair.Accuracy,
                Name = evt.AssetPair.Name,
                BaseAssetId = evt.AssetPair.BaseAssetId,
                InvertedAccuracy = evt.AssetPair.InvertedAccuracy,
                IsDisabled = evt.AssetPair.IsDisabled,
                QuotingAssetId = evt.AssetPair.QuotingAssetId,
                Source = evt.AssetPair.Source,
                Source2 = evt.AssetPair.Source2
            });
        }
    }
}
