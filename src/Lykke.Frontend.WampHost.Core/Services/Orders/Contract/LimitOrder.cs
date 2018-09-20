using System;

namespace Lykke.Frontend.WampHost.Core.Orders.Contract
{
    public class LimitOrder
    {
        public string Id { get; set; }

        public string ExternalId { get; set; }

        public string AssetPairId { get; set; }

        public string ClientId { get; set; }

        public double Volume { get; set; }

        public double Price { get; set; }

        public double? LowerLimitPrice { get; set; }

        public double? LowerPrice { get; set; }

        public double? UpperLimitPrice { get; set; }

        public double? UpperPrice { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime Registered { get; set; }

        public DateTime? LastMatchTime { get; set; }

        public double RemainingVolume { get; set; }

        public bool Straight { get; set; } = true;
    }
}
