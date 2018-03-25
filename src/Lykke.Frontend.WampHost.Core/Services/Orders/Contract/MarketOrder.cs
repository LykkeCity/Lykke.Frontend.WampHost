using System;

namespace Lykke.Frontend.WampHost.Core.Orders.Contract
{
    public class MarketOrder
    {
        public string Id { get; set; }

        public string ExternalId { get; set; }

        public string AssetPairId { get; set; }

        public string ClientId { get; set; }

        public double Volume { get; set; }

        public double? Price { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime Registered { get; set; }

        public DateTime? MatchedAt { get; set; }

        public bool Straight { get; set; }

        public double? ReservedLimitVolume { get; set; }

        public double? DustSize { get; set; }
    }
}
