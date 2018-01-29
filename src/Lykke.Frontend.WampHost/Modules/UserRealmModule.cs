using Autofac;
using Autofac.Core;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.Balances;
using WampSharp.V2;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Modules
{
    public class UserRealmModule : Module
    {
        private readonly WampHostSettings _settings;

        public UserRealmModule(WampHostSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            const string realm = "user";

            builder.Register(x => x.Resolve<IWampHost>().RealmContainer.GetRealmByName(realm));

            RegisterBalances(builder, realm);
        }

        private void RegisterBalances(ContainerBuilder builder, string realm)
        {
            builder.RegisterType<BalancesConsumer>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.BalancesRabbitMqSettings))
                .WithParameter(
                    new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(IWampHostedRealm),
                        (pi, ctx) => ctx.Resolve<IWampHost>().RealmContainer.GetRealmByName(realm)))
                .PreserveExistingDefaults();
        }
    }
}
