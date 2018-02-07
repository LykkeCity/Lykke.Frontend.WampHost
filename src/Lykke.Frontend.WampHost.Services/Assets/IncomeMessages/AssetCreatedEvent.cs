using MessagePack;

namespace Lykke.Frontend.WampHost.Services.Assets.IncomeMessages
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class AssetCreatedEvent
    {
        public Asset Asset { get; set; }
    }
}
