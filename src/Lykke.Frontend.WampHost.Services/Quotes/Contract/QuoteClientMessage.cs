using System;
using System.ComponentModel;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Domain.Quotes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Frontend.WampHost.Services.Quotes.Contract
{
    [DisplayName("Quote")]
    public class QuoteClientMessage
    {
        [DisplayName("Asset pair (BTCUSD...)")]
        [JsonProperty("a")]
        public string AssetPairId { get; set; }

        [DisplayName("Market")]
        [JsonProperty("m")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MarketType Market { get; set; }

        [DisplayName("Price type")]
        [JsonProperty("pt")]
        [JsonConverter(typeof(StringEnumConverter))]
        public QuotePriceType PriceType { get; set; }

        [DisplayName("Price")]
        [JsonProperty("p")]
        public double Price { get; set; }

        [DisplayName("Quote timestamp")]
        [JsonProperty("t")]
        public DateTime Timestamp { get; set; }
    }
}
