using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        
        public double RemainingVolume { set; get; }
        
        public bool Straight { set; get; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderType Type { set; get; }
        
        public DateTime CreatedAt { set; get; }
    }

    public enum OrderType
    {
        Limit, Market
    }
}
