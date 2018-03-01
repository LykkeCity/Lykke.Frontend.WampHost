using AutoMapper;
using Lykke.Frontend.WampHost.Services.Assets.Contracts;
using Lykke.Frontend.WampHost.Services.Assets.IncomeMessages;

namespace Lykke.Frontend.WampHost.Services.Mappers
{
    public class AssetsProfile : Profile
    {
        public AssetsProfile()
        {
            CreateMap<Asset, AssetUpdateMessage>();
            CreateMap<AssetPair, AssetPairUpdateMessage>();
        }
    }
}
