using System;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Domain.Quotes;
using Lykke.Frontend.WampHost.Core.Services.Quotes;
using Lykke.Frontend.WampHost.Services.Quotes.Contract;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Quotes
{
    [UsedImplicitly]
    public class QuotesManager : IQuotesManager
    {
        private readonly IWampHostedRealm _realm;

        public QuotesManager(IWampHostedRealm realm)
        {
            _realm = realm;
        }
        public void ProcessQuote(MarketType market, string assetPair, QuotePriceType priceType, double price, DateTime timestamp)
        {
            var topic = $"quote.{market.ToString().ToLower()}.{assetPair.ToLower()}.{priceType.ToString().ToLower()}";
            var subject = _realm.Services.GetSubject<QuoteClientMessage>(topic);

            subject.OnNext(new QuoteClientMessage
            {
                AssetPairId = assetPair,
                Market = market,
                PriceType = priceType,
                Timestamp = timestamp,
                Price = price
            });
        }
    }
}
