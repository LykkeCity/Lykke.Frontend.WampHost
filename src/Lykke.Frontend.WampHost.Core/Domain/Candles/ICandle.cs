using System;
using Lykke.Domain.Prices;

namespace Lykke.Frontend.WampHost.Core.Domain.Candles
{
    public interface ICandle
    {
        string AssetPairId { get; }
        PriceType PriceType { get; }
        TimeInterval TimeInterval { get; }
        DateTime Timestamp { get; }
        double Open { get; }
        double Close { get; }
        double High { get; }
        double Low { get; }
    }
}