using System.Linq;
using Lykke.Frontend.WampHost.Core.Services.Security;
using WampSharp.V2.Authentication;

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
            if (details.HelloDetails.AuthenticationMethods != null && details.HelloDetails.AuthenticationMethods.Contains(AuthMethods.Ticket))
            {
                return new TicketSessionAuthenticator(details, _tokenValidator, _sessionCache);
            }

            return new AnonymousWampSessionAuthenticator();
        }
    }
}
