using JetBrains.Annotations;

namespace Lykke.Frontend.WampHost.Settings
{
    [UsedImplicitly]
    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }
    }
}