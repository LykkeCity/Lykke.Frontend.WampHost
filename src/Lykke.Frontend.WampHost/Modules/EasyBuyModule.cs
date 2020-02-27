using Autofac;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.EasyBuy;

namespace Lykke.Frontend.WampHost.Modules
{
    public class EasyBuyModule : Module
    {
        private readonly WampHostSettings _settings;

        public EasyBuyModule(WampHostSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // builder.RegisterType<EasyBuyPricesSubscriber>()
            //     .As<ISubscriber>()
            //     .WithParameter(TypedParameter.From(_settings.EasyBuyRabbitMqSettings.ConnectionString))
            //     .SingleInstance();
        }
    }
}
