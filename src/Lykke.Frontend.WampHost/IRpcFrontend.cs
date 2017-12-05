using Lykke.Frontend.WampHost.Models;
using Lykke.Frontend.WampHost.Services.Documentation;
using WampSharp.V2.Rpc;

namespace Lykke.Frontend.WampHost
{
    public interface IRpcFrontend
    {
        [WampProcedure("is.alive")]
        [DocMe(Name = "is.alive", Description = "Checks service isAlive")]
        IsAliveResponse IsAlive();
    }
}