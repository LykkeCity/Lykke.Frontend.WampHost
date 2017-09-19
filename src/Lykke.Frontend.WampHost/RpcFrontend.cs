using System;
using Lykke.Frontend.WampHost.Models;
using Lykke.Frontend.WampHost.Services.Documentation;
using Microsoft.Extensions.PlatformAbstractions;
using WampSharp.V2.Rpc;

namespace Lykke.Frontend.WampHost
{
    public interface IRpcFrontend
    {
        [WampProcedure("is.alive")]
        [DocMe(Name = "is.alive", Description = "Checks service isAlive")]
        IsAliveResponse IsAlive();
    }

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