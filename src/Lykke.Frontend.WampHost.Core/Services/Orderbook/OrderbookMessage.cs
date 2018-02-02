using System;
using System.Collections.Generic;

namespace Lykke.Frontend.WampHost.Core.Services.Orderbook
{
    public class OrderbookMessage
    {
        public string AssetPair { get; set; }
        public bool IsBuy { get; set; }
        public DateTime Timestamp { get; set; }
        public List<VolumePrice> Prices { get; set; } = new List<VolumePrice>();
    }
    
    public class OrderbookModel
    {
        public string AssetPair { get; set; }
        public bool IsBuy { get; set; }
        public DateTime Timestamp { get; set; }
        public List<Level> Levels { get; set; } = new List<Level>();
    }

    public class VolumePrice
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
    }
    
    public class Level
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
    }
}
