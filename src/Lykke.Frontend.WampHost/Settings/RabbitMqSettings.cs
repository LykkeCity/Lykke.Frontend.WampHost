using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Frontend.WampHost.Settings
{
    [UsedImplicitly]
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }
    }
}
