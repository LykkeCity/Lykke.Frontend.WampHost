using Newtonsoft.Json;

namespace Lykke.Frontend.WampHost.Core.Services.Orders.OutgoingMessages
{
    public class Order
    {
        public string Id { set; get; }
        
        public string Status { set; get; }
        
        public string AssetPairId { set; get; }
        
        [JsonProperty(NullValueHandling=NullValueHandling.Include)]
        public double? Price { set; get; }
        
        public double Volume { set; get; }
        
        [JsonProperty(NullValueHandling=NullValueHandling.Include)]
        public double? RemainingVolume { set; get; }
        
        public bool Straight { set; get; }
    }
}
