using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
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
        private readonly IRabbitMqSubscribeHelper _rabbitMqSubscribeHelper;
        private readonly string _connectionString;
        private readonly MarketType _marketType;

        public CandlesSubscriber(ILog log, ICandlesManager candlesManager,
            IRabbitMqSubscribeHelper rabbitMqSubscribeHelper,
            string connectionString, MarketType marketType)
        {
            _log = log;
            _candlesManager = candlesManager;
            _rabbitMqSubscribeHelper = rabbitMqSubscribeHelper;
            _connectionString = connectionString;
            _marketType = marketType;
        }

        public void Start()
        {
            _rabbitMqSubscribeHelper.Subscribe(
                _connectionString,
                _marketType,
                "candles-v2",
                new MessagePackMessageDeserializer<CandlesUpdatedEvent>(),
                ProcessCandleAsync);
        }

        private Task ProcessCandleAsync(CandlesUpdatedEvent updatedCandles)
        {
            try
            {
                var validationErrors = ValidateCandle(updatedCandles);
                if (validationErrors.Any())
                {
                    var message = string.Join("\r\n", validationErrors);
                    _log.WriteWarning(nameof(ProcessCandleAsync), updatedCandles, message);

                    return Task.CompletedTask;
                }

                _candlesManager.ProcessCandles(updatedCandles, _marketType);
            }
            catch (Exception)
            {
                _log.WriteWarning(nameof(ProcessCandleAsync), updatedCandles, "Failed to process candle");
                throw;
            }
            
            return Task.CompletedTask;
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

            if (updatedCandles.ContractVersion.Major != Constants.ContractVersion.Major)
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
    }
}
