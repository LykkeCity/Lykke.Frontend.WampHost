using System.ComponentModel;

namespace Lykke.Frontend.WampHost.Core.Domain
{
    public enum MarketType
    {
        [DisplayName("Spot")]
        Spot,
        [DisplayName("Margin trading")]
        Mt
    }
}
