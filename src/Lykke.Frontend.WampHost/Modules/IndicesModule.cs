using Autofac;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.Indices;

namespace Lykke.Frontend.WampHost.Modules
{
    [UsedImplicitly]
    public class IndicesModule : Module
    {
        private readonly AppSettings _settings;

        public IndicesModule(AppSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IndicesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.WampHost.IndicesMqSettings.ConnectionString));
        }
    }
}
