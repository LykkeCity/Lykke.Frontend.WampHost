using System;

namespace Lykke.Frontend.WampHost.Services.EasyBuy.Contract
{
    public class PriceUpdateMessage
    {
        /// <summary>The unique identifier of the price.</summary>
        public string Id { get; set; }
        
        /// <summary>The name of asset pair.</summary>
        public string AssetPair { get; set; }

        /// <summary>The value of price.</summary>
        public decimal Value { get; set; }

        /// <summary>The maximum allowed base volume.</summary>
        public decimal BaseVolume { get; set; }

        /// <summary>The maximum allowed quote volume.</summary>
        public decimal QuoteVolume { get; set; }

        /// <summary>The date since the price valid.</summary>
        public DateTime ValidFrom { get; set; }

        /// <summary>The date until the price is valid.</summary>
        public DateTime ValidTo { get; set; }
    }
}
