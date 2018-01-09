using System;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Core.Services.Security;
using WampSharp.V2.Authentication;
using WampSharp.V2.Core.Contracts;

namespace Lykke.Frontend.WampHost.Security
{
    public class BasicTokenAuthenticator : WampSessionAuthenticator
    {
        private readonly IClientResolver _clientResolver;
        private readonly TokenAuthorizer _authorizer;

        public BasicTokenAuthenticator(
            [NotNull] WampPendingClientDetails details,
            [NotNull] IClientResolver clientResolver)
        {
            _clientResolver = clientResolver ?? throw new ArgumentNullException(nameof(clientResolver));
            _authorizer = new TokenAuthorizer();

            AuthenticationId = details.HelloDetails.AuthenticationId;
        }

        public override void Authenticate(string signature, AuthenticateExtraData extra)
        {
            var token = AuthenticationId;
            
            if (_clientResolver.ValidateAsync(token).Result)
            {
                IsAuthenticated = true;

                WelcomeDetails = new WelcomeDetails
                {
                    AuthenticationProvider = "static",
                    AuthenticationRole = "Wamp client"
                };

                Authorizer = _authorizer;
            }
            else
            {
                IsAuthenticated = false;
            }
        }

        public override string AuthenticationId { get; }

        public override string AuthenticationMethod => AuthMethods.Token;
    }
}
