using Autofac;
using Common.Log;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Security;
using Lykke.Frontend.WampHost.Core.Services.Security;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services;
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
    }
}
