using Autofac;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.Balances;

namespace Lykke.Frontend.WampHost.Modules
{
    public class BalancesModule : Module
    {
        private readonly WampHostSettings _settings;

        public BalancesModule(WampHostSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BalancesConsumer>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.BalancesRabbitMqSettings))
                .PreserveExistingDefaults();
        }
    }
}
