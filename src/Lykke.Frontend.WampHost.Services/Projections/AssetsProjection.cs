using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Services.Assets.Contracts;
using Lykke.Frontend.WampHost.Services.Assets.IncomeMessages;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Projections
{
    public class AssetsProjection
    {
        private readonly IMapper _mapper;
        private readonly ILog _log;
        private readonly ISubject<AssetUpdateMessage> _subjectAssets;
        private readonly ISubject<AssetPairUpdateMessage> _subjectAssetPairs;
        private const string AssetsTopicUri = "assets";
        private const string AssetPairsTopicUri = "assetpairs";

        public AssetsProjection(
            [NotNull] ILog log,
            [NotNull] IWampHostedRealm realm,
            [NotNull] IMapper mapper)
        {
            _log = log.CreateComponentScope(nameof(AssetsProjection));
            _subjectAssets = realm.Services.GetSubject<AssetUpdateMessage>(AssetsTopicUri);
            _subjectAssetPairs = realm.Services.GetSubject<AssetPairUpdateMessage>(AssetPairsTopicUri);
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Handle(AssetCreatedEvent evt)
        {
            _subjectAssets.OnNext(_mapper.Map<AssetUpdateMessage>(evt.Asset));
        }

        public async Task Handle(AssetUpdatedEvent evt)
        {
            _subjectAssets.OnNext(_mapper.Map<AssetUpdateMessage>(evt.Asset));
        }

        public async Task Handle(AssetPairCreatedEvent evt)
        {
            _subjectAssetPairs.OnNext(_mapper.Map<AssetPairUpdateMessage>(evt.AssetPair));
        }

        public async Task Handle(AssetPairUpdatedEvent evt)
        {
            _subjectAssetPairs.OnNext(_mapper.Map<AssetPairUpdateMessage>(evt.AssetPair));
        }
    }
}
