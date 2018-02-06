using System;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Services.Security;
using WampSharp.V2.Authentication;
using WampSharp.V2.Core.Contracts;

namespace Lykke.Frontend.WampHost.Security
{
    public class TicketSessionAuthenticator : WampSessionAuthenticator
    {
        private readonly WampPendingClientDetails _details;
        private readonly ITokenValidator _tokenValidator;
        private readonly ISessionCache _sessionCache;

        public TicketSessionAuthenticator(
            [NotNull] WampPendingClientDetails details,
            [NotNull] ITokenValidator sessionService,
            [NotNull] ISessionCache sessionCache)
        {
            _details = details ?? throw new ArgumentNullException(nameof(details));
            _tokenValidator = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _sessionCache = sessionCache ?? throw new ArgumentNullException(nameof(sessionCache));

            AuthenticationId = details.HelloDetails.AuthenticationId;
        }

        public override void Authenticate(string signature, AuthenticateExtraData extra)
        {
            if (_tokenValidator.Validate(signature))
            {
                _sessionCache.AddSessionId(signature, _details.SessionId);

                IsAuthenticated = true;

                WelcomeDetails = new WelcomeDetails
                {
                    AuthenticationRole = "Lykke client"
                };

                Authorizer = TokenAuthorizer.Instance;
            }
        }

        public override string AuthenticationId { get; }

        public override string AuthenticationMethod => AuthMethods.Ticket;

    }
}
