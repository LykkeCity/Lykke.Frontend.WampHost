using JetBrains.Annotations;
using Lykke.Service.ClientAccount.Client;

namespace Lykke.Frontend.WampHost.Core.Settings
{
    [UsedImplicitly]
    public class AppSettings
    {
        public RedisSettings RedisSettings { get; set; }
        public WampHostSettings WampHost { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public SessionServiceClientSettings SessionServiceClient { get; set; }
        public ClientAccountServiceClientSettings ClientAccountServiceClient { set; get; }
    }
}
