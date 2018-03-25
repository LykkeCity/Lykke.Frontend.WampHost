using System;

namespace Lykke.Frontend.WampHost.Core.Orders.Contract
{
    public class FeeTransfer
    {
        public string ExternalId { get; set; }

        public string FromClientId { get; set; }

        public string ToClientId { get; set; }

        public DateTime DateTime { get; set; }

        public double Volume { get; set; }

        public string Asset { get; set; }
    }
}
