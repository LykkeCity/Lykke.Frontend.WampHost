﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Orders;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Orders;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.Orders;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Frontend.WampHost.Modules
{
    public class OrdersModule : Module
    {
        private readonly AppSettings _settings;
        private readonly IServiceCollection _services;

        public OrdersModule(AppSettings settings)
        {
            _settings = settings;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LimitOrdersSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(MarketType.Spot))
                .WithParameter("connectionString", _settings.WampHost.LimitOrdersRabbitMqSettings.ConnectionString)
                .WithParameter("exchangeName", _settings.WampHost.LimitOrdersExchangeName);
            
            builder.RegisterType<MarketOrdersSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(MarketType.Spot))
                .WithParameter("connectionString", _settings.WampHost.MarketOrdersMqSettings.ConnectionString)
                .WithParameter("exchangeName", _settings.WampHost.MarketOrdersExchangeName);
            
            builder.RegisterType<OrdersConverter>()
                .As<IOrdersConverter>()
                .SingleInstance();
            
            builder.RegisterType<OrdersPublisher>()
                .As<IOrdersPublisher>()
                .SingleInstance();
            
            builder.Populate(_services);
        }
    }
}
