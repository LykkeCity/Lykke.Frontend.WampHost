using System;
using Lykke.Job.TradesConverter.Contract;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Frontend.WampHost.Services.TradesAnon.Contract
{
    public class TradeAnonClientMessage
    {
        public long Id { get; set; }
        
        public string TradeId { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public Direction Direction { get; set; }
        
        public string Asset { get; set; }
        
        public decimal Volume { get; set; }
        
        public decimal Price { get; set; }
        
        public DateTime DateTime { get; set; }
        
        public string OppositeAsset { get; set; }
        
        public decimal? OppositeVolume { get; set; }
        
        public bool? IsHidden { get; set; }
        
        public TradeAnonFee Fee { get; set; }
    }

    public class TradeAnonFee
    {
        public string Type { set; get; }
        public double? Size { set; get; }
        public string SizeType { set; get; }
    }

    public static class TradesConverter
    {
        public static TradeAnonClientMessage ToTradeAnonClientMessage(this TradeLogItem tradeLogItem)
        {
            var result = new TradeAnonClientMessage
            {
                Id = tradeLogItem.Id,
                TradeId = tradeLogItem.TradeId,
                Direction = tradeLogItem.Direction,
                Asset = tradeLogItem.Asset,
                Volume = tradeLogItem.Volume,
                Price = tradeLogItem.Price,
                DateTime = tradeLogItem.DateTime,
                OppositeAsset = tradeLogItem.OppositeAsset,
                OppositeVolume = tradeLogItem.OppositeVolume,
                IsHidden = tradeLogItem.IsHidden
            };

            if (tradeLogItem.Fee != null)
            {
                result.Fee = new TradeAnonFee
                {
                    Type = tradeLogItem.Fee.Type,
                    Size = tradeLogItem.Fee.Size,
                    SizeType = tradeLogItem.Fee.SizeType
                };
            }
            
            return result;
        }
    }
}
