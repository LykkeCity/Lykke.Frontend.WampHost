using System;
using System.Linq;
using Autofac;
using Common;
using Common.Log;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.TradesAnon;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.TradesAnon;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;

namespace Lykke.Frontend.WampHost.Modules
{
    public class TradesAnonModule : Module
    {
        private readonly AppSettings _settings;
        private readonly ILog _log;
        
        public TradesAnonModule(AppSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TradesAnonSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(MarketType.Spot))
                .WithParameter(TypedParameter.From(_settings.WampHost.ElasticRabbitMqSettings.ConnectionString));

            builder.RegisterType<TradesAnonManager>()
                .As<ITradesAnonManager>()
                .SingleInstance();
            
            builder.RegisterInstance<IAssetsService>(
                new AssetsService(new Uri(_settings.AssetsServiceClient.ServiceUrl)));
            
            builder.Register(c =>
            {
                var ctx = c.Resolve<IComponentContext>();
                return new CachedDataDictionary<string, AssetPair>(
                    async () => 
                    {
                        await _log.WriteInfoAsync("Update assetpairs cache...", "", "");
                        return (await ctx.Resolve<IAssetsService>().AssetPairGetAllAsync()).ToDictionary(itm => itm.Id);
                    });
            }).SingleInstance();
        }
    }
}
