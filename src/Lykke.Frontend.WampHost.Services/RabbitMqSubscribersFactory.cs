using System;
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
    public class RabbitMqSubscribersFactory : IRabbitMqSubscribersFactory
    {
        private static readonly IReadOnlyDictionary<MarketType, string> NamespaceMap = new Dictionary<MarketType, string>
        {
            [MarketType.Spot] = "lykke",
            [MarketType.Mt] = "lykke.mt"
        };

        private readonly ILog _log;

        public RabbitMqSubscribersFactory(ILog log)
        {
            _log = log;
        }

        public IStopable Create<TMessage>(string connectionString, MarketType market, string source, IMessageDeserializer<TMessage> deserializer, Func<TMessage, Task> handler)
        {
            var ns = NamespaceMap[market];

            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(connectionString, ns, source, ns, "wamp")
                .MakeDurable();

            return new RabbitMqSubscriber<TMessage>(settings, new DefaultErrorHandlingStrategy(_log, settings))
                .SetMessageDeserializer(deserializer)
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(handler)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .Start();
        }

        public IStopable Create<TMessage>(string connectionString, string source, IMessageDeserializer<TMessage> deserializer, Func<TMessage, Task> handler)
        {
            var settings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = connectionString,
                QueueName = $"{source}.wamp",
                ExchangeName = source,
                IsDurable = false
            };
            return new RabbitMqSubscriber<TMessage>(settings, new DefaultErrorHandlingStrategy(_log, settings))
                .SetMessageDeserializer(deserializer)
                .SetMessageReadStrategy(new MessageReadWithTemporaryQueueStrategy())
                .Subscribe(handler)
                .SetLogger(_log)
                .Start();
        }
    }
}
