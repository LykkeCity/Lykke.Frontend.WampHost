using Autofac;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.TradesAnon;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.TradesAnon;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;

namespace Lykke.Frontend.WampHost.Modules
{
    public class TradesAnonModule : Module
    {
        private readonly AppSettings _settings;

        public TradesAnonModule(AppSettings settings)
        {
            _settings = settings;
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
                Configuration = _settings.RedisSettings.Configuration,
                InstanceName = _settings.WampHost.CacheSettings.Instance
            });

            builder.RegisterInstance(tradesAnonRedisCache)
                .As<IDistributedCache>()
                .Keyed<IDistributedCache>("tradesAnonData")
                .SingleInstance();

            builder.RegisterInstance(_settings.WampHost.CacheSettings)
                .As<CacheSettings>()
                .SingleInstance();

            builder.RegisterType<TradesAnonManager>()
                .As<ITradesAnonManager>()
                .SingleInstance();
        }
    }
}
