using MessagePack;

namespace Lykke.Frontend.WampHost.Services.Assets.IncomeMessages
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class AssetPairUpdatedEvent
    {
        public AssetPair AssetPair { get; set; }
    }
}
