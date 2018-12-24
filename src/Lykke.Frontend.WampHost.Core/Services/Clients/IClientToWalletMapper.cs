using System.Threading.Tasks;

namespace Lykke.Frontend.WampHost.Core.Services.Clients
{
    public interface IClientToWalletMapper
    {
        Task<(string, string)> GetClientIdAndWalletIdAsync(string id);
    }
}
