using Autofac;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.Orderbooks.Spot;

namespace Lykke.Frontend.WampHost.Modules
{
    public class OrderBooksModule : Module
    {
        private readonly WampHostSettings _settings;

        public OrderBooksModule(WampHostSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SpotOrderbookSubscriber>()
                .As<ISubscriber>()
                .WithParameter(TypedParameter.From(_settings.MeRabbitMqSettings.ConnectionString))
                .SingleInstance();
        }
    }
}
