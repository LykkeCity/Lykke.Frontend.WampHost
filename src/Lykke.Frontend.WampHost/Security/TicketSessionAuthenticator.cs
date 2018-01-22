using System;
using JetBrains.Annotations;
using WampSharp.V2.Authentication;
using WampSharp.V2.Core.Contracts;

namespace Lykke.Frontend.WampHost.Security
{
    public class TicketSessionAuthenticator : WampSessionAuthenticator
    {
        private readonly ITokenValidator _tokenValidator;

        public TicketSessionAuthenticator(
            [NotNull] WampPendingClientDetails details,
            [NotNull] ITokenValidator sessionService)
        {
            _tokenValidator = sessionService ?? throw new ArgumentNullException(nameof(sessionService));

            AuthenticationId = details.HelloDetails.AuthenticationId;
        }

        public override void Authenticate(string signature, AuthenticateExtraData extra)
        {
            if (_tokenValidator.ValidateAsync(signature).Result)
            {
                IsAuthenticated = true;

                WelcomeDetails = new WelcomeDetails
                {
                    AuthenticationRole = "Lykke client"
                };

                Authorizer = AnonymousWampAuthorizer.Instance;
            }
        }

        public override string AuthenticationId { get; }

        public override string AuthenticationMethod => AuthMethods.Ticket;

    }
}
