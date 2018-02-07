using System;
using System.Threading.Tasks;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Frontend.WampHost.Core.Services
{
    public interface IRabbitMqSubscribeHelper
    {
        void Subscribe<TMessage>(string connectionString, MarketType market, string source, IMessageDeserializer<TMessage> deserializer, Func<TMessage, Task> handler);
    }
}
