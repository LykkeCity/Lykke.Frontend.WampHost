using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Orderbook;
using Lykke.Frontend.WampHost.Services.Quotes.Spot;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Frontend.WampHost.Services.Orderbooks.Spot
{
    [UsedImplicitly]
    public class SpotOrderbookSubscriber : ISubscriber
    {
        private readonly IOrderbookManager _orderbookManager;
        private readonly ILog _log;
        private readonly IRabbitMqSubscribersFactory _subscribersFactory;
        private readonly string _connectionString;

        private IStopable _subscriber;

        public SpotOrderbookSubscriber(
            IOrderbookManager orderbookManager,
            IRabbitMqSubscribersFactory subscribersFactory, 
            string connectionString,
            ILog log
            )
        {
            _orderbookManager = orderbookManager;
            _log = log;
            _subscribersFactory = subscribersFactory;
            _connectionString = connectionString;
        }

        public void Start()
        {
            _subscriber = _subscribersFactory.Create(
                _connectionString, 
                MarketType.Spot, 
                "orderbook",
                new JsonMessageDeserializer<OrderbookMessage>(),
                ProcessOrderbookAsync);
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        private async Task ProcessOrderbookAsync(OrderbookMessage orderbookMessage)
        {
            try
            {
                var validationErrors = ValidateOrderbook(orderbookMessage);

                if (validationErrors.Any())
                {
                    var message = string.Join("\r\n", validationErrors);
                    await _log.WriteWarningAsync(nameof(SpotQuotesSubscriber), nameof(ProcessOrderbookAsync), orderbookMessage?.ToJson(), message);

                    return;
                }

                _orderbookManager.ProcessOrderbook(orderbookMessage);
            }
            catch (Exception)
            {
                await _log.WriteWarningAsync(nameof(SpotQuotesSubscriber), nameof(ProcessOrderbookAsync), orderbookMessage?.ToJson(), "Failed to process orderbook");
                throw;
            }
        }

        private static IReadOnlyCollection<string> ValidateOrderbook(OrderbookMessage orderbookMessage)
        {
            var errors = new List<string>();

            if (orderbookMessage == null)
            {
                errors.Add("Orderbook is null.");
            }
            else
            {
                if (string.IsNullOrEmpty(orderbookMessage.AssetPair))
                {
                    errors.Add("Empty 'AssetPair'");
                }

                //TODO: validate prices if needed
            }

            return errors;
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }
    }
}
