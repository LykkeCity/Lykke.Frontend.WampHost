using System.Linq;
using WampSharp.V2.Authentication;

namespace Lykke.Frontend.WampHost.Security
{
    public class WampSessionAuthenticatorFactory : IWampSessionAuthenticatorFactory
    {
        private readonly ITokenValidator _tokenValidator;

        public WampSessionAuthenticatorFactory(ITokenValidator tokenValidator)
        {
            _tokenValidator = tokenValidator;
        }

        public IWampSessionAuthenticator GetSessionAuthenticator(
            WampPendingClientDetails details,
            IWampSessionAuthenticator transportAuthenticator)
        {
            if (details.HelloDetails.AuthenticationMethods.Contains(AuthMethods.Ticket))
            {
                return new TicketSessionAuthenticator(details, _tokenValidator);
            }

            return new AnonymousWampSessionAuthenticator();
        }
    }
}
