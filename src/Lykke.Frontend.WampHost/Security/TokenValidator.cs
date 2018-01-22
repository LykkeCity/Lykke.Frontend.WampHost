using System;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Service.Session;

namespace Lykke.Frontend.WampHost.Security
{
    class TokenValidator : ITokenValidator
    {
        private readonly ILog _log;
        private readonly IClientsSessionsRepository _sessionService;

        public TokenValidator(
            [NotNull] ILog log,
            [NotNull] IClientsSessionsRepository sessionService)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        public async Task<bool> ValidateAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            return await GetClientId(token) != null;
        }

        private async Task<string> GetClientId(string token)
        {
            try
            {
                var sessionModel = await _sessionService.GetAsync(token);
                return sessionModel?.ClientId;
            }
            catch (Exception exception)
            {
                await _log.WriteWarningAsync(nameof(TokenValidator), nameof(GetClientId), exception.Message, "Failed to get session by token");
                return null;
            }
        }
    }
}
