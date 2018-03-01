using MessagePack;

namespace Lykke.Frontend.WampHost.Services.Assets.IncomeMessages
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class AssetUpdatedEvent
    {
        public Asset Asset { get; set; }
    }
}
