using System;
using System.Threading.Tasks;
using Common;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Frontend.WampHost.Core.Services
{
    public interface IRabbitMqSubscribersFactory
    {
        IStopable Create<TMessage>(string connectionString, MarketType market, string source, IMessageDeserializer<TMessage> deserializer, Func<TMessage, Task> handler);
    }
}
