using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.TradesAnon;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.TradesAnon;
using Lykke.Service.Assets.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Frontend.WampHost.Modules
{
    public class TradesAnonModule : Module
    {
        private readonly AppSettings _settings;
        private readonly IServiceCollection _services;

        public TradesAnonModule(AppSettings settings)
        {
            _settings = settings;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TradesAnonSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(MarketType.Spot))
                .WithParameter(TypedParameter.From(_settings.WampHost.TradesAnonMqSettings.ConnectionString));

            var tradesAnonRedisCache = new RedisCache(new RedisCacheOptions
            {
                Configuration = _settings.WampHost.RedisSettings.Configuration,
                InstanceName = _settings.WampHost.RedisSettings.Instance
            });
            
            builder.RegisterInstance(tradesAnonRedisCache)
                .As<IDistributedCache>()
                .Keyed<IDistributedCache>("tradesAnonData")
                .SingleInstance();
            
            builder.RegisterType<TradesAnonManager>()
                .As<ITradesAnonManager>()
                .SingleInstance();

            var cacheExpirationPeriod = TimeSpan.FromMinutes(5);
            _services.RegisterAssetsClient(AssetServiceSettings.Create(
                new Uri(_settings.AssetsServiceClient.ServiceUrl),
                cacheExpirationPeriod));

            builder.Populate(_services);
        }
    }
}
