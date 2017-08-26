using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Frontend.WampHost.Core;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Services;
using Microsoft.Extensions.DependencyInjection;
using WampSharp.V2;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Modules
{
    public class HostModule : Module
    {
        private readonly WampHostSettings _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public HostModule(WampHostSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings)
                .SingleInstance();

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            var host = new WampSharp.V2.WampHost();
            var realm = host.RealmContainer.GetRealmByName("main");

            builder.RegisterInstance(host)
                .As<IWampHost>();

            builder.RegisterInstance(realm)
                .As<IWampHostedRealm>();

            builder.Populate(_services);
        }
    }
}
