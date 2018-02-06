namespace Lykke.Frontend.WampHost.Core.Services.Security
{
    public interface ISessionCache
    {
        long[] GetSessionIds(string clientId);
        void AddSessionId(string token, long sessionId);
        bool TryRemoveSessionId(long sessionId);
    }
}
