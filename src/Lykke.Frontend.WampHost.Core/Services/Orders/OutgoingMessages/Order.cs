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

        public static OrderStatus GetOrderStatus(string status)
        {
            var parsed = MeOrderStatus.TryParse(status, true, out MeOrderStatus meStatusCode);

            if (parsed)
            {
                switch (meStatusCode)
                {
                    case MeOrderStatus.InOrderBook:
                        return OrderStatus.InOrderBook;
                    case MeOrderStatus.Cancelled:
                        return OrderStatus.Cancelled;
                    case MeOrderStatus.Matched:
                        return OrderStatus.Matched;
                    case MeOrderStatus.Processing:
                        return OrderStatus.Processing;
                    default:
                        return OrderStatus.Rejected;
                }
            }
            else
            {
                throw new ArgumentException($"Status {status} did not match any of the expected ME orders' status codes");
            }
        }

        public static string GetOrderRejectReason(string status)
        {
            return GetOrderStatus(status) == OrderStatus.Rejected ? status : null;
        }
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
        InvalidPrice
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
