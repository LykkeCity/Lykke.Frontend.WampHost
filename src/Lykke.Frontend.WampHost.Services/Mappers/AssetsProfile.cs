using AutoMapper;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Services.Assets.Contracts;
using Lykke.Frontend.WampHost.Services.Assets.IncomeMessages;
using Lykke.Service.Assets.Contract.Events;

namespace Lykke.Frontend.WampHost.Services.Mappers
{
    [UsedImplicitly]
    public class AssetsProfile : Profile
    {
        public AssetsProfile()
        {            
            CreateMap<AssetPair, AssetPairUpdateMessage>();

            CreateMap<AssetCreatedEvent, AssetUpdateMessage>();
            CreateMap<AssetUpdatedEvent, AssetUpdateMessage>();
        }
    }
}
