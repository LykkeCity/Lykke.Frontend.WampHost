using JetBrains.Annotations;

namespace Lykke.Frontend.WampHost.Settings
{
    [UsedImplicitly]
    public class RabbitMqSettings
    {
        public string ConnectionString { get; set; }
    }
}