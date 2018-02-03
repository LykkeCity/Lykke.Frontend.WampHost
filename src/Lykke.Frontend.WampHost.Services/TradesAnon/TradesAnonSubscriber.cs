using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Candles;
using Lykke.Frontend.WampHost.Core.Services.TradesAnon;
using Lykke.Job.TradesConverter.Contract;
using Lykke.RabbitMqBroker.Subscriber;
using WampSharp.V2;
using WampSharp.V2.Core.Contracts;

namespace Lykke.Frontend.WampHost.Services.TradesAnon
{
    public class TradesAnonSubscriber : ISubscriber
    {
        private IStopable _subscriber;
        private readonly ILog _log;
        private readonly ITradesAnonManager _tradesAnonManager;
        private readonly IRabbitMqSubscribersFactory _subscribersFactory;
        private readonly string _connectionString;
        private readonly MarketType _marketType;
        
        public TradesAnonSubscriber(ILog log, ITradesAnonManager tradesAnonManager, IRabbitMqSubscribersFactory subscribersFactory, string connectionString, MarketType marketType)
        {
            _log = log;
            _tradesAnonManager = tradesAnonManager;
            _subscribersFactory = subscribersFactory;
            _connectionString = connectionString;
            _marketType = marketType;
        }
        
        public void Start()
        {
            _subscriber = _subscribersFactory.Create(
                _connectionString,
                _marketType,
                "tradelog",
                new MessagePackMessageDeserializer<List<TradeLogItem>>(),
                ProcessTradeAsync);
        }

        private async Task ProcessTradeAsync(List<TradeLogItem> messages)
        {
            if (!messages.Any())
                return;
            
            try
            {
                foreach (var tradeLogItem in messages)
                {
                    await _tradesAnonManager.ProcessTrade(tradeLogItem, _marketType);
                }
            }
            catch (Exception ex)
            {
                _log.WriteWarning(nameof(ProcessTradeAsync), messages, "Failed to process trade", ex);
                throw;
            }

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }
    }
}
