using System;
using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Models;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Frontend.WampHost
{
    [UsedImplicitly]
    public class RpcFrontend : IRpcFrontend
    {
        public IsAliveResponse IsAlive()
        {
            return new IsAliveResponse
            {
                Version = PlatformServices.Default.Application.ApplicationVersion,
                Env = Environment.GetEnvironmentVariable("ENV_INFO")
            };
        }
    }
}
