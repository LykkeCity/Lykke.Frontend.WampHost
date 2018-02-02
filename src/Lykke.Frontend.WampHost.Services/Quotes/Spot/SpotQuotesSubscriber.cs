using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Domain.Quotes;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Quotes;
using Lykke.Job.QuotesProducer.Contract;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Frontend.WampHost.Services.Quotes.Spot
{
    [UsedImplicitly]
    public class SpotQuotesSubscriber : ISubscriber
    {
        private readonly ILog _log;
        private readonly IQuotesManager _quotesManager;
        private readonly IRabbitMqSubscribersFactory _subscribersFactory;
        private readonly string _connectionString;

        private IStopable _subscriber;

        public SpotQuotesSubscriber(ILog log, IQuotesManager quotesManager, IRabbitMqSubscribersFactory subscribersFactory, string connectionString)
        {
            _log = log;
            _quotesManager = quotesManager;
            _subscribersFactory = subscribersFactory;
            _connectionString = connectionString;
        }

        public void Start()
        {
            _subscriber = _subscribersFactory.Create(
                _connectionString, 
                MarketType.Spot, 
                "quotefeed",
                new JsonMessageDeserializer<QuoteMessage>(),
                ProcessQuoteAsync);
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        private async Task ProcessQuoteAsync(QuoteMessage quote)
        {
            try
            {
                var validationErrors = ValidateQuote(quote);
                if (validationErrors.Any())
                {
                    var message = string.Join("\r\n", validationErrors);
                    _log.WriteWarning( nameof(ProcessQuoteAsync), quote, message);

                    return;
                }

                _quotesManager.ProcessQuote(
                    MarketType.Spot,
                    quote.AssetPair, 
                    quote.IsBuy ? QuotePriceType.Bid : QuotePriceType.Ask, 
                    quote.Price, 
                    quote.Timestamp);
            }
            catch (Exception)
            {
                _log.WriteWarning(nameof(ProcessQuoteAsync), quote, "Failed to process quote");
                throw;
            }
        }

        private static IReadOnlyCollection<string> ValidateQuote(QuoteMessage quote)
        {
            var errors = new List<string>();

            if (quote == null)
            {
                errors.Add("Quote is null.");
            }
            else
            {
                if (string.IsNullOrEmpty(quote.AssetPair))
                {
                    errors.Add("Empty 'AssetPair'");
                }
                if (quote.Timestamp.Kind != DateTimeKind.Utc)
                {
                    errors.Add($"Invalid 'Timestamp' Kind (UTC is required): '{quote.Timestamp.Kind}'");
                }
                if (quote.Price <= 0)
                {
                    errors.Add($"Not positive price: '{quote.Price}'");
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
