using System;
using Lykke.Frontend.WampHost.Core.Services.Orderbook;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Orderbooks
{
    public class OrderbookManager : IOrderbookManager
    {
        private readonly IWampHostedRealm _realm;

        public OrderbookManager(IWampHostedRealm realm)
        {
            _realm = realm;
        }

        public void ProcessOrderbook(OrderbookMessage orderbookMessage)
        {
            var topic = $"orderbook.{orderbookMessage.AssetPair.ToLower()}.{(orderbookMessage.IsBuy ? "buy" : "sell")}";
            var subject = _realm.Services.GetSubject<OrderbookMessage>(topic);
            
            ProcessPrices(orderbookMessage);
            
            subject.OnNext(orderbookMessage);
        }

        private void ProcessPrices(OrderbookMessage message)
        {
            foreach (var price in message.Prices)
            {
                price.Volume = Math.Abs(price.Volume);
            }
        }
    }
}
