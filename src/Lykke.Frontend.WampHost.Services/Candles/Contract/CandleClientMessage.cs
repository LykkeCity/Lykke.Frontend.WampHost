using System;
using Lykke.Job.CandlesProducer.Contract;
using System.ComponentModel;
using Lykke.Frontend.WampHost.Core.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Frontend.WampHost.Services.Candles.Contract
{
    [DisplayName("Candle")]
    public class CandleClientMessage
    {
        [DisplayName("Asset pair (BTCUSD...)")]
        [JsonProperty("a")]
        public string AssetPairId { get; set; }

        [DisplayName("Market")]
        [JsonProperty("m")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MarketType MarketType { get; set; }

        [DisplayName("Price type")]
        [JsonProperty("p")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CandlePriceType PriceType { get; set; }

        [DisplayName("Interval")]
        [JsonProperty("i")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CandleTimeInterval TimeInterval { get; set; }

        [DisplayName("Candle open timestamp")]
        [JsonProperty("t")]
        public DateTime Timestamp { get; set; }

        [DisplayName("Open price")]
        [JsonProperty("o")]
        public double Open { get; set; }

        [DisplayName("Close price")]
        [JsonProperty("c")]
        public double Close { get; set; }

        [DisplayName("High price")]
        [JsonProperty("h")]
        public double High { get; set; }

        [DisplayName("Low price")]
        [JsonProperty("l")]
        public double Low { get; set; }

        [DisplayName("Trading volume")]
        [JsonProperty("v")]
        public double TradingVolume { get; set; }

        [DisplayName("Opposite trading volume")]
        [JsonProperty("ov")]
        public double OppositeTradingVolume { get; set; }
    }
}
