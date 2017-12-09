using System;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Domain.Quotes;

namespace Lykke.Frontend.WampHost.Core.Services.Quotes
{
    public interface IQuotesManager
    {
        void ProcessQuote(MarketType market, string assetPair, QuotePriceType priceType, double price, DateTime timestamp);
    }
}
