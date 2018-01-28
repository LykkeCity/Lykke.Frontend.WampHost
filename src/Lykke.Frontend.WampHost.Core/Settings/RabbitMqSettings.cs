using JetBrains.Annotations;

namespace Lykke.Frontend.WampHost.Core.Settings
{
    [UsedImplicitly]
    public class RabbitMqSettings
    {
        public string ConnectionString { get; set; }
    }
}
