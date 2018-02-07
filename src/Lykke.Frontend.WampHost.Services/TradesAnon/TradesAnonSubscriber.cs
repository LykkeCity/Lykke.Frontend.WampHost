using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.TradesAnon;
using Lykke.Job.TradesConverter.Contract;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Frontend.WampHost.Services.TradesAnon
{
    public class TradesAnonSubscriber : ISubscriber
    {
        private readonly ILog _log;
        private readonly ITradesAnonManager _tradesAnonManager;
        private readonly IRabbitMqSubscribeHelper _rabbitMqSubscribeHelper;
        private readonly string _connectionString;
        private readonly MarketType _marketType;

        public TradesAnonSubscriber(
            [NotNull] ILog log,
            [NotNull] ITradesAnonManager tradesAnonManager,
            [NotNull] IRabbitMqSubscribeHelper rabbitMqSubscribeHelper,
            [NotNull] string connectionString,
            MarketType marketType)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _tradesAnonManager = tradesAnonManager ?? throw new ArgumentNullException(nameof(tradesAnonManager));
            _rabbitMqSubscribeHelper = rabbitMqSubscribeHelper ?? throw new ArgumentNullException(nameof(rabbitMqSubscribeHelper));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _marketType = marketType;
        }

        public void Start()
        {
            _rabbitMqSubscribeHelper.Subscribe(
                connectionString: _connectionString,
                market: _marketType,
                source: "tradelog",
                deserializer: new MessagePackMessageDeserializer<List<TradeLogItem>>(),
                handler: ProcessTradeAsync);
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
    }
}
