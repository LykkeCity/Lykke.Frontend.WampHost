using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Trades;
using Lykke.Job.TradesConverter.Contract;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Frontend.WampHost.Services.Trades
{
    [UsedImplicitly]
    public class TradesSubscriber : ISubscriber
    {
        private readonly ILog _log;
        private readonly ITradesManager _tradesManager;
        private readonly IRabbitMqSubscribersFactory _subscribersFactory;
        private readonly string _connectionString;
        private IStopable _subscriber;
        
        public TradesSubscriber(
            ITradesManager tradesManager,
            IRabbitMqSubscribersFactory subscribersFactory, 
            string connectionString,
            ILog log
            )
        {
            _tradesManager = tradesManager;
            _log = log;
            _subscribersFactory = subscribersFactory;
            _connectionString = connectionString;
        }

        public void Start()
        {
            _subscriber = _subscribersFactory.Create(
                _connectionString, 
                MarketType.Spot, 
                "tradelog",
                new MessagePackMessageDeserializer<List<TradeLogItem>>(),
                ProcessTradeAsync);
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        private async Task ProcessTradeAsync(List<TradeLogItem> messages)
        {
            try
            {
               _tradesManager.ProcessTrade(messages);
            }
            catch (Exception ex)
            {
                await _log.WriteWarningAsync(nameof(TradesSubscriber), nameof(ProcessTradeAsync), messages?.ToJson(), "Failed to process trade", ex);
                throw;
            }
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }
    }
}
