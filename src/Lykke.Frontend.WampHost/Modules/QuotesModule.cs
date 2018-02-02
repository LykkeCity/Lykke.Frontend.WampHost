using Autofac;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Quotes;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.Quotes;
using Lykke.Frontend.WampHost.Services.Quotes.Mt;
using Lykke.Frontend.WampHost.Services.Quotes.Spot;

namespace Lykke.Frontend.WampHost.Modules
{
    public class QuotesModule : Module
    {
        private readonly WampHostSettings _settings;

        public QuotesModule(WampHostSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
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
