using System.Linq;
using WampSharp.V2.Authentication;
using WampSharp.V2.Core.Contracts;

namespace Lykke.Frontend.WampHost.Security
{
    public class AnonymousWampAuthorizer : IWampAuthorizer
    {
        public static AnonymousWampAuthorizer Instance = new AnonymousWampAuthorizer();

        private readonly string[] _topicsWithAuth = {"trades"};

        public bool CanRegister(RegisterOptions options, string procedure) => false;

        public bool CanCall(CallOptions options, string procedure) => true;

        public bool CanPublish(PublishOptions options, string topicUri) => false;

        public bool CanSubscribe(SubscribeOptions options, string topicUri) => !_topicsWithAuth.Any(topicUri.StartsWith);
    }
}
