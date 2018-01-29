using JetBrains.Annotations;

namespace Lykke.Frontend.WampHost.Core.Settings
{
    [UsedImplicitly]
    public class WampHostSettings
    {
        public DbSettings Db { get; set; }

        public RabbitMqSettings RabbitMqSettings { get; set; }
        public RabbitMqSettings MtRabbitMqSettings { get; set; }

        public RabbitMqSettings SpotQuotesRabbitMqSettings { get; set; }
        public RabbitMqSettings MtQuotesRabbitMqSettings { get; set; }

        public RabbitMqSettings BalancesRabbitMqSettings { get; set; }
    }
}
