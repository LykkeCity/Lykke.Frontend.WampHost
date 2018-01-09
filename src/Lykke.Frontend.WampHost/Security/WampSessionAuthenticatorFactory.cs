using System.Linq;
using Lykke.Frontend.WampHost.Core.Services.Security;
using WampSharp.V2.Authentication;

namespace Lykke.Frontend.WampHost.Security
{
    public class WampSessionAuthenticatorFactory : IWampSessionAuthenticatorFactory
    {
        private readonly ITokenValidator _tokenValidator;
        private readonly IClientResolver _clientResolver;

        public WampSessionAuthenticatorFactory(
            ITokenValidator tokenValidator,
            IClientResolver clientResolver)
        {
            _tokenValidator = tokenValidator;
            _clientResolver = clientResolver;
        }

        public IWampSessionAuthenticator GetSessionAuthenticator(
            WampPendingClientDetails details,
            IWampSessionAuthenticator transportAuthenticator)
        {
            if (details.HelloDetails.AuthenticationMethods.Contains(AuthMethods.Ticket))
            {
                return new TicketSessionAuthenticator(details, _tokenValidator, _clientResolver);
            }

            return new AnonymousWampSessionAuthenticator();
        }
    }
}
