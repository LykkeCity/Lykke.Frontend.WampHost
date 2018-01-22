using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Frontend.WampHost.Core.Services.Orderbook;

namespace Lykke.Frontend.WampHost.Services.Extensions
{
    internal static class OrderbookModelsExtensions
    {
        internal static OrderbookModel ToModel(this OrderbookMessage src)
        {
            return new OrderbookModel
            {
                AssetPair = src.AssetPair,
                IsBuy = src.IsBuy,
                Timestamp = src.Timestamp,
                Levels = src.Prices.ProcessPrices()
            };
        }

        private static List<Level> ProcessPrices(this IEnumerable<VolumePrice> prices)
        {
            return prices.Select(price => new Level
            {
                ClientId = price.ClientId,
                Id = price.Id,
                Price = price.Price,
                Volume = Math.Abs(price.Volume)
            }).ToList();
        }
    }
}
