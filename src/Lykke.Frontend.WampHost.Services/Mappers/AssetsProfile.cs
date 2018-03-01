using AutoMapper;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Services.Assets.Contracts;
using Lykke.Frontend.WampHost.Services.Assets.IncomeMessages;

namespace Lykke.Frontend.WampHost.Services.Mappers
{
    [UsedImplicitly]
    public class AssetsProfile : Profile
    {
        public AssetsProfile()
        {
            CreateMap<Asset, AssetUpdateMessage>();
            CreateMap<AssetPair, AssetPairUpdateMessage>();
        }
    }
}
