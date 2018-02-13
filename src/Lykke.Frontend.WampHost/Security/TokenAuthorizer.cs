using WampSharp.V2.Authentication;
using WampSharp.V2.Core.Contracts;

namespace Lykke.Frontend.WampHost.Security
{
    public class TokenAuthorizer : IWampAuthorizer
    {
        public static TokenAuthorizer Instance = new TokenAuthorizer();

        public bool CanRegister(RegisterOptions options, string procedure) => false;

        public bool CanCall(CallOptions options, string procedure) => false;

        public bool CanPublish(PublishOptions options, string topicUri) => false;

        public bool CanSubscribe(SubscribeOptions options, string topicUri) => true;
    }
}
