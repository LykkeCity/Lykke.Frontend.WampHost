using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Mt;

namespace Lykke.Frontend.WampHost.Settings
{
    [UsedImplicitly]
    public class WampHostSettings
    {
        public DbSettings Db { get; set; }

        public RabbitMqSettings RabbitMqSettings { get; set; }
        public RabbitMqSettings MtRabbitMqSettings { get; set; }

        public RabbitMqSettings SpotQuotesRabbitMqSettings { get; set; }
        public RabbitMqSettings MtQuotesRabbitMqSettings { get; set; }
        
        public MtSubscriberSettings MtSubscriberSettings { get; set; }
    }
}
