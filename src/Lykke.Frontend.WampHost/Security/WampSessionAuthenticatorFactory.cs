using WampSharp.V2.Authentication;

namespace Lykke.Frontend.WampHost.Security
{
    public class WampSessionAuthenticatorFactory : IWampSessionAuthenticatorFactory
    {
        public IWampSessionAuthenticator GetSessionAuthenticator(
            WampPendingClientDetails details,
            IWampSessionAuthenticator transportAuthenticator)
        {
            return new AnonymousWampSessionAuthenticator();
        }
    }
}
