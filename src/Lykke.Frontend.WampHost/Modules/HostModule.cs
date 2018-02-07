using Autofac;
using Common.Log;
using Lykke.Frontend.WampHost.Core.Mt;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Security;
using Lykke.Frontend.WampHost.Core.Services.Security;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services;
using Lykke.Frontend.WampHost.Services.Mt;
using Lykke.Frontend.WampHost.Services.Security;
using Lykke.Service.Session;
using WampSharp.V2;
using WampSharp.V2.Authentication;

namespace Lykke.Frontend.WampHost.Modules
{
    public class HostModule : Module
    {
        private readonly AppSettings _settings;
        private readonly ILog _log;
        private readonly string _env;

        public HostModule(AppSettings settings, ILog log, string env)
        {
            _settings = settings;
            _log = log;
            _env = env;
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

            builder.RegisterType<RabbitMqSubscribeHelper>()
                .As<IRabbitMqSubscribeHelper>()
                .WithParameter(TypedParameter.From(_env))
                .SingleInstance();

            builder.RegisterType<ClientResolver>()
                .As<ITokenValidator>()
                .As<IClientResolver>()
                .SingleInstance();

            builder.RegisterClientSessionService(_settings.SessionServiceClient.SessionServiceUrl, _log);

            RegisterWampCommon(builder);

            RegisterMt(builder);
        }

        private void RegisterMt(ContainerBuilder builder)
        {
            builder.RegisterType<MtRabbitMqHandler>().As<IMtRabbitMqHandler>().SingleInstance();
            builder.RegisterType<MtSubscriber>().As<ISubscriber>()
                .WithParameter(TypedParameter.From(_settings.WampHost.MtSubscriberSettings))
                .SingleInstance();
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

            const string realm = "prices";
            builder.Register(x => x.Resolve<IWampHost>().RealmContainer.GetRealmByName(realm));
        }
    }
}
