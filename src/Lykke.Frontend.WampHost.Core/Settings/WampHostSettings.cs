using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Frontend.WampHost.Core.Settings
{
    [UsedImplicitly]
    public class WampHostSettings
    {
        public OAuthSettings OAuthSettings { get; set; }

        public DbSettings Db { get; set; }
        
        public string LimitOrdersExchangeName { set; get; }
        public string MarketOrdersExchangeName { set; get; }

        public RabbitMqSettings RabbitMqSettings { get; set; }
        public RabbitMqSettings MeRabbitMqSettings { get; set; }
        public RabbitMqSettings ElasticRabbitMqSettings { get; set; }
        public RabbitMqSettings SpotQuotesRabbitMqSettings { get; set; }
        public RabbitMqSettings BalancesRabbitMqSettings { get; set; }
        public RabbitMqSettings SagasRabbitMqSettings { get; set; }        
        public RabbitMqSettings TradesAnonMqSettings { get; set; }
        public RabbitMqSettings LimitOrdersRabbitMqSettings { get; set; }        
        public RabbitMqSettings MarketOrdersMqSettings { get; set; }       
        public RabbitMqSettings IndicesMqSettings { get; set; }
        public CacheSettings CacheSettings { get; set; }
    }
}
