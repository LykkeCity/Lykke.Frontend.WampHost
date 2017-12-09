using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Frontend.WampHost.Services.Candles
{
    [UsedImplicitly]
    public class CandlesSubscriber : ISubscriber
    {   
        private readonly ILog _log;
        private readonly ICandlesManager _candlesManager;
        private readonly IRabbitMqSubscribersFactory _subscribersFactory;
        private readonly string _connectionString;
        private readonly MarketType _marketType;

        private IStopable _subscriber;

        public CandlesSubscriber(ILog log, ICandlesManager candlesManager, IRabbitMqSubscribersFactory subscribersFactory, string connectionString, MarketType marketType)
        {
            _log = log;
            _candlesManager = candlesManager;
            _subscribersFactory = subscribersFactory;
            _connectionString = connectionString;
            _marketType = marketType;
        }

        public void Start()
        {
            _subscriber = _subscribersFactory.Create(
                _connectionString, 
                _marketType, 
                "candles", 
                new JsonMessageDeserializer<CandleMessage>(),
                ProcessCandleAsync);
        }

        public void Stop()
        {
            _subscriber?.Stop();
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
            catch (Exception)
            {
                await _log.WriteWarningAsync(nameof(CandlesSubscriber), nameof(ProcessCandleAsync), candle.ToJson(), "Failed to process candle");
                throw;
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
            _subscriber?.Dispose();
        }
    }
}
