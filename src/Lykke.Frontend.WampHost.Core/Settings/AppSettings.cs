﻿using JetBrains.Annotations;
using Lykke.Service.Session;

namespace Lykke.Frontend.WampHost.Core.Settings
{
    [UsedImplicitly]
    public class AppSettings
    {
        public WampHostSettings WampHost { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public SessionServiceClientSettings SessionServiceClient { get; set; }
    }
}