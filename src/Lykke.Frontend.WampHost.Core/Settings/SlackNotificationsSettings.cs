using JetBrains.Annotations;

namespace Lykke.Frontend.WampHost.Core.Settings
{
    [UsedImplicitly]
    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }
    }
}
