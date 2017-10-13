using System.Threading.Tasks;

namespace Lykke.Frontend.WampHost.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}