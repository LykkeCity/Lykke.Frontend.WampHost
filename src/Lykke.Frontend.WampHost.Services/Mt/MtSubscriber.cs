using System;
using System.Threading.Tasks;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Mt;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.RabbitMqBroker.Subscriber;
using MarginTrading.Contract.BackendContracts;
using MarginTrading.Contract.ClientContracts;
using MarginTrading.Contract.RabbitMqMessageModels;

namespace Lykke.Frontend.WampHost.Services.Mt
{
    public class MtSubscriber : ISubscriber
    {
        private readonly IRabbitMqSubscribeHelper _rabbitMqSubscribeHelper;
        private readonly IMtRabbitMqHandler _mtRabbitMqHandler;
        private readonly MtSubscriberSettings _settings;

        public MtSubscriber(IRabbitMqSubscribeHelper rabbitMqSubscribeHelper, IMtRabbitMqHandler mtRabbitMqHandler,
            MtSubscriberSettings settings)
        {
            _rabbitMqSubscribeHelper = rabbitMqSubscribeHelper;
            _mtRabbitMqHandler = mtRabbitMqHandler;
            _settings = settings;
        }

        public void Start()
        {
            // Account changes

            Subscribe<AccountChangedMessage>(_settings.LiveRabbitMqConnString,
                _settings.Exchanges.AccountChanged,
                _mtRabbitMqHandler.ProcessAccountChanged);

            Subscribe<AccountChangedMessage>(_settings.DemoRabbitMqConnString,
                _settings.Exchanges.AccountChanged,
                _mtRabbitMqHandler.ProcessAccountChanged);

            // Order changes

            Subscribe<OrderContract>(_settings.LiveRabbitMqConnString,
                _settings.Exchanges.OrderChanged,
                _mtRabbitMqHandler.ProcessOrderChanged);

            Subscribe<OrderContract>(_settings.DemoRabbitMqConnString,
                _settings.Exchanges.OrderChanged,
                _mtRabbitMqHandler.ProcessOrderChanged);

            // Stopout

            Subscribe<AccountStopoutBackendContract>(_settings.LiveRabbitMqConnString,
                _settings.Exchanges.AccountStopout,
                _mtRabbitMqHandler.ProcessAccountStopout);

            Subscribe<AccountStopoutBackendContract>(_settings.DemoRabbitMqConnString,
                _settings.Exchanges.AccountStopout,
                _mtRabbitMqHandler.ProcessAccountStopout);

            // User updates

            Subscribe<UserUpdateEntityBackendContract>(_settings.LiveRabbitMqConnString,
                _settings.Exchanges.UserUpdates,
                _mtRabbitMqHandler.ProcessUserUpdates);

            Subscribe<UserUpdateEntityBackendContract>(_settings.DemoRabbitMqConnString,
                _settings.Exchanges.UserUpdates,
                _mtRabbitMqHandler.ProcessUserUpdates);

            // Trades

            Subscribe<TradeContract>(_settings.LiveRabbitMqConnString,
                _settings.Exchanges.Trades,
                _mtRabbitMqHandler.ProcessTrades);

            Subscribe<TradeContract>(_settings.DemoRabbitMqConnString,
                _settings.Exchanges.Trades,
                _mtRabbitMqHandler.ProcessTrades);
        }

        private void Subscribe<TMessage>(string connectionString, string exchangeName, Action<TMessage> handler)
        {
            _rabbitMqSubscribeHelper.Subscribe(connectionString, MarketType.Mt, exchangeName,
                new JsonMessageDeserializer<TMessage>(), m =>
                {
                    handler(m);
                    return Task.CompletedTask;
                });
        }
    }
}
