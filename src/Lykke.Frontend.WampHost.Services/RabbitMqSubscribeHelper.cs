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
using Microsoft.Extensions.PlatformAbstractions;

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
        private readonly IConsole _consoleWriter;
        private readonly string _env;

        public RabbitMqSubscribeHelper(ILog log, IConsole consoleWriter, [CanBeNull] string env)
        {
            _log = log;
            _consoleWriter = consoleWriter;
            _env = env ?? "DefaultEnv";
        }

        public void Dispose()
        {
            foreach (var stopable in _stopables)
            {
                stopable.Stop();
            }
        }

        public void Subscribe<TMessage>(string connectionString, MarketType market, string source,
            IMessageDeserializer<TMessage> deserializer, Func<TMessage, Task> handler)
        {
            var ns = NamespaceMap[market];

            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(connectionString, ns, source, ns,
                    $"{PlatformServices.Default.Application.ApplicationName}.{_env}");
            settings.DeadLetterExchangeName = null;

            var rabbitMqSubscriber =
                new RabbitMqSubscriber<TMessage>(settings, new DefaultErrorHandlingStrategy(_log, settings))
                    .SetMessageDeserializer(deserializer)
                    .CreateDefaultBinding()
                    .Subscribe(handler)
                    .SetLogger(_log)
                    .SetConsole(_consoleWriter)
                    .Start();

            _stopables.Add(rabbitMqSubscriber);
        }
    }
}
