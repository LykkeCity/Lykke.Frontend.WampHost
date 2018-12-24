using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Frontend.WampHost.Services
{
    [UsedImplicitly]
    public class RabbitMqSubscribeHelper : IRabbitMqSubscribeHelper, IDisposable
    {
        private readonly ConcurrentBag<IStopable> _stopables = new ConcurrentBag<IStopable>();

        private static readonly IReadOnlyDictionary<MarketType, string> NamespaceMap =
            new Dictionary<MarketType, string>
            {
                [MarketType.Spot] = "lykke",
                [MarketType.Mt] = "lykke.mt"
            };

        private readonly ILog _log;
        private readonly string _env;

        public RabbitMqSubscribeHelper(ILog log, [CanBeNull] string env)
        {
            _log = log;
            _env = env ?? "DefaultEnv";
        }

        public void Dispose()
        {
            foreach (var stopable in _stopables)
            {
                stopable.Stop();
            }
        }

        public void Subscribe<TMessage>(
            string connectionString,
            MarketType market,
            string source,
            IMessageDeserializer<TMessage> deserializer,
            Func<TMessage, Task> handler)
        {
            Subscribe(
                connectionString,
                market,
                source,
                null,
                deserializer,
                handler);
        }

        public void Subscribe<TMessage>(
            string connectionString,
            MarketType market,
            string source,
            string context,
            IMessageDeserializer<TMessage> deserializer,
            Func<TMessage, Task> handler)
        {
            var ns = NamespaceMap[market];

            var applicationName = "wamphost";
            var endpoint = context == null ? string.Empty : $".{context}";
            endpoint = $"{applicationName}{endpoint}.{_env}";
            var settings = RabbitMqSubscriptionSettings.ForSubscriber(connectionString, ns, source, ns, endpoint);
            settings.DeadLetterExchangeName = null;

            var rabbitMqSubscriber =
                new RabbitMqSubscriber<TMessage>(settings, new DefaultErrorHandlingStrategy(_log, settings))
                    .SetMessageDeserializer(deserializer)
                    .CreateDefaultBinding()
                    .Subscribe(handler)
                    .SetLogger(_log)
                    .Start();

            _stopables.Add(rabbitMqSubscriber);
        }
    }
}
