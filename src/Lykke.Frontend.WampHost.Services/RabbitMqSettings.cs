namespace Lykke.Frontend.WampHost.Services
{
    public class RabbitMqSettings
    {
        public string ConnectionString { get; set; }
        public string ExchangeName { get; set; }
        public string DeadLetterExchangeName { get; set; }        
    }    
}