﻿using WampSharp.V2.Authentication;
using WampSharp.V2.Core.Contracts;

namespace Lykke.Frontend.WampHost.Security
{
    public class AnonymousWampAuthorizer : IWampAuthorizer
    {
        public static AnonymousWampAuthorizer Instance = new AnonymousWampAuthorizer();

        public bool CanRegister(RegisterOptions options, string procedure)
        {
            return false;
        }

        public bool CanCall(CallOptions options, string procedure)
        {
            return true;
        }

        public bool CanPublish(PublishOptions options, string topicUri)
        {
            return false;
        }

        public bool CanSubscribe(SubscribeOptions options, string topicUri)
        {
            return string.IsNullOrEmpty(options?.Match) || options.Match == WampMatchPattern.Exact;
        }
    }
}