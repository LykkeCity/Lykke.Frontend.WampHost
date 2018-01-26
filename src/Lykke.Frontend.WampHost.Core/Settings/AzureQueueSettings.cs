using JetBrains.Annotations;

namespace Lykke.Frontend.WampHost.Core.Settings
{
    [UsedImplicitly]
    public class AzureQueueSettings
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }
}
