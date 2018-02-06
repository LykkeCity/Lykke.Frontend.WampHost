using System.Linq;
using Lykke.Frontend.WampHost.Core.Services.Security;
using WampSharp.V2.Authentication;
using WampSharp.V2.Core.Contracts;

namespace Lykke.Frontend.WampHost.Security
{
    public class WampSessionAuthenticatorFactory : IWampSessionAuthenticatorFactory
    {
        private readonly ITokenValidator _tokenValidator;
        private readonly ISessionCache _sessionCache;

        public WampSessionAuthenticatorFactory(
            ITokenValidator tokenValidator,
            ISessionCache sessionCache)
        {
            _tokenValidator = tokenValidator;
            _sessionCache = sessionCache;
        }

        public IWampSessionAuthenticator GetSessionAuthenticator(
            WampPendingClientDetails details,
            IWampSessionAuthenticator transportAuthenticator)
        {
            // todo: change hardcoded realm name into realm collection resolving
            if (details.Realm != "prices")
                throw new WampAuthenticationException(new AbortDetails { Message = "unknown realm" });

            if (details.HelloDetails.AuthenticationMethods != null && details.HelloDetails.AuthenticationMethods.Contains(AuthMethods.Ticket))
            {
                return new TicketSessionAuthenticator(details, _tokenValidator, _sessionCache);
            }

            return new AnonymousWampSessionAuthenticator();
        }
    }
}
