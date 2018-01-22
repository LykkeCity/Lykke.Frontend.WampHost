using System.Threading.Tasks;

namespace Lykke.Frontend.WampHost.Security
{
    public interface ITokenValidator
    {
        Task<bool> ValidateAsync(string token);
    }
}
