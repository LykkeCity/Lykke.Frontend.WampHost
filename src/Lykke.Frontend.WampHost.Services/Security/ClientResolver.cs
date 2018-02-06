using System;
using System.Collections.Generic;
using System.Linq;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Services.Security;
using Lykke.Service.Session;
using Microsoft.Extensions.Caching.Memory;

namespace Lykke.Frontend.WampHost.Services.Security
{
    public class ClientResolver : ITokenValidator, ISessionCache
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

        private string GetClientId(string token)
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

        private static readonly long[] ZeroSessionsValue = new long[0];
        public long[] GetSessionIds(string clientId)
        {
            if (_cache.TryGetValue(clientId, out long[] sessionIds))
            {
                return sessionIds;
            }

            return ZeroSessionsValue;
        }

        public void AddSessionId(string token, long sessionId)
        {
            var clientId = GetClientId(token);
            _cache.Set(sessionId, clientId, _sessionCacheOptions);

            if (_cache.TryGetValue(clientId, out long[] sessionIds))
            {
                // working with HashSet is not effective, but more readable than working with Array; type 'long[]' is stored for performance needs
                var sessions = new HashSet<long>(sessionIds) { sessionId };
                _cache.Set(clientId, sessions.ToArray(), _sessionCacheOptions);
            }
            else
            {
                _cache.Set(clientId, new[] { sessionId }, _sessionCacheOptions);
            }
        }

        public bool TryRemoveSessionId(long sessionId)
        {
            if (_cache.TryGetValue(sessionId, out string clientId))
            {
                _cache.Remove(sessionId);

                // working with HashSet is not effective, but more readable than working with Array; type 'long[]' is stored for performance needs
                var sessionIds = _cache.Get<long[]>(clientId);
                var sessions = new HashSet<long>(sessionIds);
                sessions.Remove(sessionId);
                _cache.Set(clientId, sessions.ToArray(), _sessionCacheOptions);
            }

            return false;
        }
    }
}
