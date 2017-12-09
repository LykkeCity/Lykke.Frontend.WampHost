using System;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Domain.Quotes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Frontend.WampHost.Services.Quotes.Contract
{
    public class QuoteClientMessage
    {
        [JsonProperty("a")]
        public string AssetPairId { get; set; }

        [JsonProperty("m")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MarketType Market { get; set; }

        [JsonProperty("pt")]
        [JsonConverter(typeof(StringEnumConverter))]

        public QuotePriceType PriceType { get; set; }

        [JsonProperty("p")]
        public double Price { get; set; }

        [JsonProperty("t")]
        public DateTime Timestamp { get; set; }
    }
}
