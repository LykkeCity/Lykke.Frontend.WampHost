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

        [JsonProperty(NullValueHandling=NullValueHandling.Include)]
        public double? LowerLimitPrice { set; get; }

        [JsonProperty(NullValueHandling=NullValueHandling.Include)]
        public double? LowerPrice { set; get; }

        [JsonProperty(NullValueHandling=NullValueHandling.Include)]
        public double? UpperLimitPrice { set; get; }

        [JsonProperty(NullValueHandling=NullValueHandling.Include)]
        public double? UpperPrice { set; get; }
        
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
        Replaced,
        Pending
    }

    public enum OrderStatus
    {
        InOrderBook,
        Processing,
        Matched,
        Cancelled,
        Rejected,
        Pending
    }

    public enum OrderType
    {
        Limit, Market, StopLimit
    }

    public enum OrderAction
    {
        Buy, Sell
    }
}
