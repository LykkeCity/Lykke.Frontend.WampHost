﻿using Autofac;
using Common.Log;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Clients;
using Lykke.Frontend.WampHost.Security;
using Lykke.Frontend.WampHost.Core.Services.Security;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services;
using Lykke.Frontend.WampHost.Services.Clients;
using Lykke.Frontend.WampHost.Services.Security;
using Lykke.Service.ClientAccount.Client;
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
                .As<ISessionCache>()
                .SingleInstance();

            builder.RegisterType<OAuthTokenValidator>()
                .WithParameter(TypedParameter.From(new OAuth2IntrospectionOptions
                {
                    ClientId = _settings.WampHost.OAuthSettings.ClientId,
                    ClientSecret = _settings.WampHost.OAuthSettings.ClientSecret,
                    Authority = _settings.WampHost.OAuthSettings.Authority
                }))
                .As<IOAuthTokenValidator>()
                .SingleInstance();

            builder.RegisterType<ClientToWalletMapper>()
                .As<IClientToWalletMapper>()
                .SingleInstance();

            builder.RegisterClientSessionService(_settings.SessionServiceClient.SessionServiceUrl, _log);

            builder.RegisterLykkeServiceClient(_settings.ClientAccountServiceClient.ServiceUrl);

            RegisterWampCommon(builder);
        }

        private static void RegisterWampCommon(ContainerBuilder builder)
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
