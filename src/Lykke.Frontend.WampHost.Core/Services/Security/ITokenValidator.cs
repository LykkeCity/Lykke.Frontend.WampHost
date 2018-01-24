
namespace Lykke.Frontend.WampHost.Core.Services.Security
{
    public interface ITokenValidator
    {
        bool Validate(string token);
    }
}
