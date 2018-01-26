using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Frontend.WampHost.Core.Settings
{
    [UsedImplicitly]
    public class RabbitMqSettings
    {
        public string ConnectionString { get; set; }
        [Optional]
        public string ExchangeName { get; set; }
    }
}
