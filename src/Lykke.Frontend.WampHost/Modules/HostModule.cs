using Autofac;
using Common.Log;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Candles;
using Lykke.Frontend.WampHost.Security;
using Lykke.Frontend.WampHost.Core.Services.Quotes;
using Lykke.Frontend.WampHost.Services;
using Lykke.Frontend.WampHost.Services.Candles;
using Lykke.Frontend.WampHost.Settings;
using Lykke.Frontend.WampHost.Services.Quotes;
using Lykke.Frontend.WampHost.Services.Quotes.Mt;
using Lykke.Frontend.WampHost.Services.Quotes.Spot;
using Lykke.Service.Session;
using WampSharp.V2;
using WampSharp.V2.Authentication;

namespace Lykke.Frontend.WampHost.Modules
{
    public class HostModule : Module
    {
        private readonly AppSettings _settings;
        private readonly WampHostSettings _hostSettings;
        private readonly ILog _log;

        public HostModule(AppSettings settings, ILog log)
        {
            _settings = settings;
            _hostSettings = settings.WampHost;
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

            builder.RegisterType<TokenValidator>()
                .As<ITokenValidator>()
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
        }

        private void RegisterCandles(ContainerBuilder builder)
        {
            builder.RegisterType<CandlesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(MarketType.Spot))
                .WithParameter(TypedParameter.From(_hostSettings.RabbitMqSettings.ConnectionString))
                .PreserveExistingDefaults();

            builder.RegisterType<CandlesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(MarketType.Mt))
                .WithParameter(TypedParameter.From(_hostSettings.MtRabbitMqSettings.ConnectionString))
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
                .WithParameter(TypedParameter.From(_hostSettings.SpotQuotesRabbitMqSettings.ConnectionString))
                .PreserveExistingDefaults();

            builder.RegisterType<MtQuotesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_hostSettings.MtQuotesRabbitMqSettings.ConnectionString))
                .PreserveExistingDefaults();

            builder.RegisterType<QuotesManager>()
                .As<IQuotesManager>()
                .SingleInstance();
        }
    }
}
