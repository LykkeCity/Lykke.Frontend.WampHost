using Autofac;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.Trades;

namespace Lykke.Frontend.WampHost.Modules
{
    public class TradesModule : Module
    {
        private readonly WampHostSettings _settings;

        public TradesModule(WampHostSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TradesSubscriber>()
                .As<ISubscriber>()
                .WithParameter(TypedParameter.From(_settings.ElasticRabbitMqSettings.ConnectionString))
                .SingleInstance();
        }
    }
}
