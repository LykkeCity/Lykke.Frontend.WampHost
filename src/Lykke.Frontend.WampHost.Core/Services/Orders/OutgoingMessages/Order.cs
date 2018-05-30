using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Frontend.WampHost.Core.Services.Orders.OutgoingMessages
{
    public class Order
    {
        public string Id { set; get; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderStatus Status { set; get; }
        
        [JsonProperty(NullValueHandling=NullValueHandling.Include)]
        public string RejectReason { set; get; }
        
        public string AssetPairId { set; get; }
        
        [JsonProperty(NullValueHandling=NullValueHandling.Include)]
        public double? Price { set; get; }
        
        public double Volume { set; get; }
        
        public double RemainingVolume { set; get; }
        
        public bool Straight { set; get; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderType Type { set; get; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderAction OrderAction { set; get; }
        
        public DateTime CreateDateTime { set; get; }
    }
    
    public enum MeOrderStatus
    {
        InOrderBook,
        Processing,
        Matched,
        NotEnoughFunds,
        ReservedVolumeGreaterThanBalance,
        NoLiquidity,
        UnknownAsset,
        DisabledAsset,
        Cancelled,
        LeadToNegativeSpread,
        InvalidFee,
        TooSmallVolume,
        InvalidPrice,
        Replaced
    }

    public enum OrderStatus
    {
        InOrderBook,
        Processing,
        Matched,
        Cancelled,
        Rejected
    }

    public enum OrderType
    {
        Limit, Market
    }

    public enum OrderAction
    {
        Buy, Sell
    }
}
