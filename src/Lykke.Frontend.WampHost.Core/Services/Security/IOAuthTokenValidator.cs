using System.Threading.Tasks;

namespace Lykke.Frontend.WampHost.Services.Security
{
    public interface IOAuthTokenValidator
    {
        Task<string> GetClientId(string token);
    }
}
