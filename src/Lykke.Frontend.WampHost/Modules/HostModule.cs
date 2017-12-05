using Autofac;
using Common.Log;
using Lykke.Frontend.WampHost.Core.Domain.Candles;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Candles;
using Lykke.Frontend.WampHost.Security;
using Lykke.Frontend.WampHost.Core.Services.Quotes;
using Lykke.Frontend.WampHost.Security;
using Lykke.Frontend.WampHost.Services;
using Lykke.Frontend.WampHost.Services.Candles;
using Lykke.Frontend.WampHost.Settings;
using Lykke.Frontend.WampHost.Services.Quotes;
using Lykke.Frontend.WampHost.Services.Quotes.Mt;
using Lykke.Frontend.WampHost.Services.Quotes.Spot;
using Lykke.Frontend.WampHost.Settings;
using WampSharp.V2;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Modules
{
    public class HostModule : Module
    {
        private readonly WampHostSettings _settings;
        private readonly ILog _log;

        public HostModule(WampHostSettings settings, ILog log)
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

            var host = RegisterWampCommon(builder);
            
            RegisterCandles(builder, host);
        }

        private static WampAuthenticationHost RegisterWampCommon(ContainerBuilder builder)
        {
            var host = new WampAuthenticationHost(new WampSessionAuthenticatorFactory());

            builder.RegisterInstance(host)
                .As<IWampHost>();

            builder.RegisterType<RpcFrontend>()
                .As<IRpcFrontend>()
                .SingleInstance();

            return host;
        }

        private void RegisterPrices(ContainerBuilder builder, WampAuthenticationHost host)
        {
            var realm = host.RealmContainer.GetRealmByName("prices");

            builder.RegisterInstance(realm)
                .As<IWampHostedRealm>();

            RegisterCandles(builder);
            RegisterQuotes(builder);
        }

        private void RegisterCandles(ContainerBuilder builder)
        {
            builder.RegisterType<CandlesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(MarketType.Spot))
                .WithParameter(TypedParameter.From(_settings.RabbitMqSettings.ConnectionString))
                .PreserveExistingDefaults();

            builder.RegisterType<CandlesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(MarketType.Mt))
                .WithParameter(TypedParameter.From(_settings.MtRabbitMqSettings.ConnectionString))
                .PreserveExistingDefaults();

            builder.RegisterType<CandlesManager>()
                .As<ICandlesManager>()
                .SingleInstance();

        private void RegisterQuotes(ContainerBuilder builder)
        {
            builder.RegisterType<SpotQuotesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.SpotQuotesRabbitMqSettings.ConnectionString))
                .PreserveExistingDefaults();

            builder.RegisterType<MtQuotesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.MtQuotesRabbitMqSettings.ConnectionString))
                .PreserveExistingDefaults();

            builder.RegisterType<QuotesManager>()
                .As<IQuotesManager>()
                .SingleInstance();
        }
    }
}
