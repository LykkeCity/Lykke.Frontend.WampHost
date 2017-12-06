using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain.Candles;
using Lykke.Frontend.WampHost.Core.Services.Candles;
using Lykke.Job.CandlesProducer.Contract;
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
                .CreateForSubscriber(_rabbitMqSettings.ConnectionString, ns, "candles", ns, "wamp")
            _subscriber = _subscribersFactory.Create(
                _connectionString, 
                _marketType, 
                "candles-v2", 
                new JsonMessageDeserializer<CandleMessage>(),
                ProcessCandleAsync);
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        private async Task ProcessCandleAsync(CandlesUpdatedEvent updatedCandles)
        {
            try
            {
                var validationErrors = ValidateCandle(updatedCandles);
                if (validationErrors.Any())
                {
                    var message = string.Join("\r\n", validationErrors);
                    await _log.WriteWarningAsync(nameof(CandlesSubscriber), nameof(ProcessCandleAsync), updatedCandles.ToJson(), message);

                    return;
                }

                _candlesManager.ProcessCandles(updatedCandles, _marketType);
            }
            catch (Exception)
            {
                await _log.WriteWarningAsync(nameof(CandlesSubscriber), nameof(ProcessCandleAsync), updatedCandles.ToJson(), "Failed to process candle");
                throw;
            }
        }

        private static IReadOnlyCollection<string> ValidateCandle(CandlesUpdatedEvent updatedCandles)
        {
            var errors = new List<string>();

            if (updatedCandles == null)
            {
                errors.Add($"'{nameof(updatedCandles)}' is null.");

                return errors;
            }

            if (updatedCandles.ContractVersion == null)
            {
                errors.Add("Contract version is not specified");

                return errors;
            }

            if (updatedCandles.ContractVersion.Major != Job.CandlesProducer.Contract.Constants.ContractVersion.Major)
            {
                errors.Add("Unsupported contract version");

                return errors;
            }

            if (updatedCandles.Candles == null || !updatedCandles.Candles.Any())
            {
                errors.Add("Candles is empty");

                return errors;
            }

            for (var i = 0; i < updatedCandles.Candles.Count; ++i)
            {
                var candle = updatedCandles.Candles[i];

                if (string.IsNullOrWhiteSpace(candle.AssetPairId))
                {
                    errors.Add($"Empty 'AssetPair' in the candle {i}");
                }

                if (candle.CandleTimestamp.Kind != DateTimeKind.Utc)
                {
                    errors.Add($"Invalid 'CandleTimestamp' Kind (UTC is required) in the candle {i}");
                }

                if (candle.TimeInterval == CandleTimeInterval.Unspecified)
                {
                    errors.Add($"Invalid 'TimeInterval' in the candle {i}");
                }

                if (candle.PriceType == CandlePriceType.Unspecified)
                {
                    errors.Add($"Invalid 'PriceType' in the candle {i}");
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
