using System;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Services.Security;
using Lykke.Service.Session;
using Microsoft.Extensions.Caching.Memory;

namespace Lykke.Frontend.WampHost.Services.Security
{
    public class ClientResolver : ITokenValidator, IClientResolver
    {
        private readonly ILog _log;
        private readonly IMemoryCache _cache;
        private readonly IClientsSessionsRepository _sessionService;

        private readonly MemoryCacheEntryOptions _tokenCacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
        private readonly MemoryCacheEntryOptions _sessionCacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(14));

        public ClientResolver(
            [NotNull] ILog log,
            [NotNull] IClientsSessionsRepository sessionService,
            [NotNull] IMemoryCache cache)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public bool Validate(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            return GetClientId(token) != null;
        }

        public string GetClientId(string token)
        {
            if (_cache.TryGetValue(token, out string clientId))
            {
                return clientId;
            }

            try
            {
                var sessionModel = _sessionService.GetAsync(token).Result;
                clientId = sessionModel?.ClientId;
            }
            catch (Exception exception)
            {
                _log.WriteError(nameof(ClientResolver), nameof(GetClientId), exception);
                return null;
            }

            if (clientId != null)
            {
                _cache.Set(token, clientId, _tokenCacheOptions);
            }

            return clientId;
        }

        public string GetNotificationId(string clientId)
        {
            if (_cache.TryGetValue(clientId, out string notificationId))
            {
                return notificationId;
            }

            return null;
        }

        public void SetNotificationId(string token, string notificationId)
        {
            var clientId = GetClientId(token);
            _cache.Set(clientId, notificationId, _sessionCacheOptions);
        }
    }
}
