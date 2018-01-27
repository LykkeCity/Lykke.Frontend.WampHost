namespace Lykke.Frontend.WampHost.Core.Services.Security
{
    public interface IClientResolver
    {
        string GetClientId(string token);
        string GetNotificationId(string clientId);
        void SetNotificationId(string token, string notificationId);
    }
}
