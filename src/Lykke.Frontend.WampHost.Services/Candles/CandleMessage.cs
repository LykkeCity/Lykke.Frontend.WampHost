using System;
using Lykke.Domain.Prices;
using Lykke.Frontend.WampHost.Core.Domain.Candles;
using Newtonsoft.Json;

namespace Lykke.Frontend.WampHost.Services.Candles
{
    public class CandleMessage : ICandle
    {
        [JsonProperty("a")]
        public string AssetPairId { get; set; }

        [JsonProperty("p")]
        public PriceType PriceType { get; set; }

        [JsonProperty("i")]
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