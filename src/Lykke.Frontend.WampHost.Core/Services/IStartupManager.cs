using System.Threading.Tasks;

namespace Lykke.Frontend.WampHost.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}