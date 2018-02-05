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
using Lykke.Frontend.WampHost.Services.Quotes.Mt.Messages;
using Lykke.Frontend.WampHost.Services.Quotes.Spot;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Frontend.WampHost.Services.Quotes.Mt
{
    [UsedImplicitly]
    public class MtQuotesSubscriber : ISubscriber
    {
        private readonly ILog _log;
        private readonly IQuotesManager _quotesManager;
        private readonly IRabbitMqSubscribeHelper _rabbitMqSubscribeHelper;
        private readonly string _connectionString;

        public MtQuotesSubscriber(ILog log, IQuotesManager quotesManager, IRabbitMqSubscribeHelper rabbitMqSubscribeHelper, string connectionString)
        {
            _log = log;
            _quotesManager = quotesManager;
            _rabbitMqSubscribeHelper = rabbitMqSubscribeHelper;
            _connectionString = connectionString;
        }

        public void Start()
        {
            _rabbitMqSubscribeHelper.Subscribe(
                _connectionString, 
                MarketType.Mt, 
                "pricefeed",
                new JsonMessageDeserializer<MtQuoteMessage>(),
                ProcessQuoteAsync);
        }

        private Task ProcessQuoteAsync(MtQuoteMessage quote)
        {
            try
            {
                var validationErrors = ValidateQuote(quote);
                if (validationErrors.Any())
                {
                    var message = string.Join("\r\n", validationErrors);
                    _log.WriteWarning(nameof(ProcessQuoteAsync), quote.ToJson(), message);
                    return Task.CompletedTask;
                }

                _quotesManager.ProcessQuote(
                    MarketType.Mt,
                    quote.Instrument,
                    QuotePriceType.Bid,
                    quote.Bid,
                    quote.Date);

                _quotesManager.ProcessQuote(
                    MarketType.Mt,
                    quote.Instrument,
                    QuotePriceType.Ask,
                    quote.Ask,
                    quote.Date);
            }
            catch (Exception)
            {
                _log.WriteWarning(nameof(ProcessQuoteAsync), quote.ToJson(), "Failed to process quote");
                throw;
            }
            
            return Task.CompletedTask;
        }

        private static IReadOnlyCollection<string> ValidateQuote(MtQuoteMessage quote)
        {
            var errors = new List<string>();

            if (quote == null)
            {
                errors.Add("Quote is null.");
            }
            else
            {
                if (string.IsNullOrEmpty(quote.Instrument))
                {
                    errors.Add("Empty 'AssetPair'");
                }
                if (quote.Date.Kind != DateTimeKind.Utc)
                {
                    errors.Add($"Invalid 'Timestamp' Kind (UTC is required): '{quote.Date.Kind}'");
                }
                if (quote.Ask <= 0)
                {
                    errors.Add($"Not positive ask price: '{quote.Ask}'");
                }
                if (quote.Bid <= 0)
                {
                    errors.Add($"Not positive bid price: '{quote.Bid}'");
                }
            }

            return errors;
        }
    }
}
