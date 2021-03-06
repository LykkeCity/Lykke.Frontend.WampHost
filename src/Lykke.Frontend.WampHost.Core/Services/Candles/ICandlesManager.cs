﻿using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Job.CandlesProducer.Contract;

namespace Lykke.Frontend.WampHost.Core.Services.Candles
{
    public interface ICandlesManager
    {
        void ProcessCandles(CandlesUpdatedEvent updatedCandles, MarketType market);
    }
}
