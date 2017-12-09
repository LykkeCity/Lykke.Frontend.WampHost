using JetBrains.Annotations;

namespace Lykke.Frontend.WampHost.Settings
{
    [UsedImplicitly]
    public class AzureQueueSettings
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }
}