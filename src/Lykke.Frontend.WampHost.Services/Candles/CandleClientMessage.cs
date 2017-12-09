using System;
using Lykke.Domain.Prices;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Domain.Candles;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Frontend.WampHost.Services.Candles
{
    public class CandleClientMessage : ICandle
    {
        [JsonProperty("a")]
        public string AssetPairId { get; set; }

        [JsonProperty("m")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MarketType MarketType { get; set; }

        [JsonProperty("p")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PriceType PriceType { get; set; }

        [JsonProperty("i")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TimeInterval TimeInterval { get; set; }

        [JsonProperty("t")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("o")]
        public double Open { get; set; }

        [JsonProperty("c")]
        public double Close { get; set; }

        [JsonProperty("h")]
        public double High { get; set; }

        [JsonProperty("l")]
        public double Low { get; set; }
    }
}
