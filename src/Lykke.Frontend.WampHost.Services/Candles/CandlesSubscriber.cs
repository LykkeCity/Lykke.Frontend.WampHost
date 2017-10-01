using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Common;
using Common.Log;
using Lykke.Frontend.WampHost.Core.Domain.Candles;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Frontend.WampHost.Services.Candles
{
    public class CandlesSubscriber : ICandlesSubscriber
    {   
        private Dictionary<MarketType, string> _namespaceMap = new Dictionary<MarketType, string>
        {
            [MarketType.Spot] = "lykke",
            [MarketType.Mt] = "lykke.mt"
        };

        private readonly ILog _log;
        private readonly ICandlesManager _candlesManager;
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly MarketType _marketType;

        private RabbitMqSubscriber<CandleMessage> _subscriber;

        public CandlesSubscriber(ILog log, ICandlesManager candlesManager, RabbitMqSettings rabbitMqSettings, MarketType marketType)
        {
            _log = log;
            _candlesManager = candlesManager;
            _rabbitMqSettings = rabbitMqSettings;
            _marketType = marketType;
        }

        public void Start()
        {
            var ns = _namespaceMap[_marketType];

            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_rabbitMqSettings.ConnectionString, ns, "candles", ns, "wamp")
                .MakeDurable();

            try
            {
                _subscriber = new RabbitMqSubscriber<CandleMessage>(settings,
                        new ResilientErrorHandlingStrategy(_log, settings,
                            retryTimeout: TimeSpan.FromSeconds(10),
                            next: new DeadQueueErrorHandlingStrategy(_log, settings)))
                    .SetMessageDeserializer(new JsonMessageDeserializer<CandleMessage>())
                    .SetMessageReadStrategy(new MessageReadQueueStrategy())
                    .Subscribe(ProcessCandleAsync)
                    .CreateDefaultBinding()
                    .SetLogger(_log)
                    .Start();
            }
            catch (Exception ex)
            {
                _log.WriteErrorAsync(nameof(CandlesSubscriber), nameof(Start), null, ex).Wait();
                throw;
            }
        }

        public void Stop()
        {
            _subscriber.Stop();
        }

        private async Task ProcessCandleAsync(CandleMessage candle)
        {
            try
            {
                var validationErrors = ValidateQuote(candle);
                if (validationErrors.Any())
                {
                    var message = string.Join("\r\n", validationErrors);
                    await _log.WriteWarningAsync(nameof(CandlesSubscriber), nameof(ProcessCandleAsync), candle.ToJson(), message);

                    return;
                }

                await _candlesManager.ProcessCandleAsync(candle, _marketType);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(CandlesSubscriber), nameof(ProcessCandleAsync), null, ex);
            }
        }

        private static IReadOnlyCollection<string> ValidateQuote(CandleMessage candle)
        {
            var errors = new List<string>();

            if (candle == null)
            {
                errors.Add("Argument 'Order' is null.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(candle.AssetPairId))
                {
                    errors.Add("Empty 'AssetPair'");
                }
                if (candle.Timestamp.Kind != DateTimeKind.Utc)
                {
                    errors.Add($"Invalid 'Timestamp' Kind (UTC is required): '{candle.Timestamp.Kind}'");
                }
            }

            return errors;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}