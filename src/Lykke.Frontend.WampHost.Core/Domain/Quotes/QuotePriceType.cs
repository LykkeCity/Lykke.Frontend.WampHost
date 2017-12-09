using System.ComponentModel;

namespace Lykke.Frontend.WampHost.Core.Domain.Quotes
{
    public enum QuotePriceType
    {
        [DisplayName("Ask price")]
        Ask,
        [DisplayName("Bid price")]
        Bid
    }
}
