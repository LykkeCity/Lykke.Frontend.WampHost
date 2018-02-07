using JetBrains.Annotations;

namespace Lykke.Frontend.WampHost.Core.Services.Security
{
    public interface IClientResolver
    {
        [CanBeNull] string GetClientId(string token);
        [CanBeNull] string GetNotificationId(string clientId);
        void SetNotificationId(string token, string notificationId);
    }
}
