namespace Lykke.Frontend.WampHost.Core.Mt
{
    public class MtSubscriberSettings
    {
        public string LiveRabbitMqConnString { get; set; }
        public string DemoRabbitMqConnString { get; set; }
        public MtExchangesNamesSettings Exchanges { get; set; }
    }
}
