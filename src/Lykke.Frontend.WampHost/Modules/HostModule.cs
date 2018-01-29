using Autofac;
using Common.Log;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Candles;
using Lykke.Frontend.WampHost.Core.Services.Orderbook;
using Lykke.Frontend.WampHost.Core.Services.Quotes;
using Lykke.Frontend.WampHost.Core.Services.Security;
using Lykke.Frontend.WampHost.Security;
using Lykke.Frontend.WampHost.Services;
using Lykke.Frontend.WampHost.Services.Candles;
using Lykke.Frontend.WampHost.Services.Orderbooks;
using Lykke.Frontend.WampHost.Services.Orderbooks.Spot;
using Lykke.Frontend.WampHost.Settings;
using Lykke.Frontend.WampHost.Services.Quotes;
using Lykke.Frontend.WampHost.Services.Quotes.Mt;
using Lykke.Frontend.WampHost.Services.Quotes.Spot;
using Lykke.Frontend.WampHost.Services.Security;
using Lykke.Frontend.WampHost.Services.Trades;
using Lykke.Service.Session;
using WampSharp.V2;
using WampSharp.V2.Authentication;

namespace Lykke.Frontend.WampHost.Modules
{
    public class HostModule : Module
    {
        private readonly AppSettings _settings;
        private readonly ILog _log;

        public HostModule(AppSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .SingleInstance();

            builder.RegisterType<RabbitMqSubscribersFactory>()
                .As<IRabbitMqSubscribersFactory>();

            builder.RegisterType<ClientResolver>()
                .As<ITokenValidator>()
                .As<IClientResolver>()
                .SingleInstance();

            builder.RegisterClientSessionService(_settings.SessionServiceClient.SessionServiceUrl, _log);

            RegisterWampCommon(builder);

            RegisterPrices(builder);
        }

        private void RegisterWampCommon(ContainerBuilder builder)
        {
            builder.RegisterType<WampSessionAuthenticatorFactory>()
                .As<IWampSessionAuthenticatorFactory>()
                .SingleInstance();

            builder.RegisterType<WampAuthenticationHost>()
                .As<IWampHost>()
                .SingleInstance();

            builder.RegisterType<RpcFrontend>()
                .As<IRpcFrontend>()
                .SingleInstance();
        }

        private void RegisterPrices(ContainerBuilder builder)
        {
            builder.Register(x => x.Resolve<IWampHost>().RealmContainer.GetRealmByName("prices"))
                .SingleInstance();

            RegisterCandles(builder);
            RegisterQuotes(builder);
            RegisterOrderbooks(builder);
            RegisterTrades(builder);
        }

        private void RegisterCandles(ContainerBuilder builder)
        {
            builder.RegisterType<CandlesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(MarketType.Spot))
                .WithParameter(TypedParameter.From(_settings.WampHost.RabbitMqSettings.ConnectionString))
                .PreserveExistingDefaults();

            builder.RegisterType<CandlesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(MarketType.Mt))
                .WithParameter(TypedParameter.From(_settings.WampHost.MtRabbitMqSettings.ConnectionString))
                .PreserveExistingDefaults();

            builder.RegisterType<CandlesManager>()
                .As<ICandlesManager>()
                .SingleInstance();
        }

        private void RegisterQuotes(ContainerBuilder builder)
        {
            builder.RegisterType<SpotQuotesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.WampHost.SpotQuotesRabbitMqSettings.ConnectionString))
                .PreserveExistingDefaults();

            builder.RegisterType<MtQuotesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.WampHost.MtQuotesRabbitMqSettings.ConnectionString))
                .PreserveExistingDefaults();

            builder.RegisterType<QuotesManager>()
                .As<IQuotesManager>()
                .SingleInstance();
        }

        private void RegisterOrderbooks(ContainerBuilder builder)
        {
            builder.RegisterType<SpotOrderbookSubscriber>()
                .As<ISubscriber>()
                .WithParameter(TypedParameter.From(_settings.WampHost.MeRabbitMqSettings.ConnectionString))
                .SingleInstance();
        }

        private void RegisterTrades(ContainerBuilder builder)
        {
            builder.RegisterType<TradesSubscriber>()
                .As<ISubscriber>()
                .WithParameter(TypedParameter.From(_settings.WampHost.ElasticRabbitMqSettings.ConnectionString))
                .SingleInstance();
        }
    }
}
