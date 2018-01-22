using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Frontend.WampHost.Core.Services.Orderbook;
using Lykke.Frontend.WampHost.Services.Extensions;
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
            var subject = _realm.Services.GetSubject<OrderbookModel>(topic);
            
            subject.OnNext(orderbookMessage.ToModel());
        }
    }
}
