using MarginTrading.Contract.BackendContracts;
using MarginTrading.Contract.ClientContracts;
using MarginTrading.Contract.RabbitMqMessageModels;

namespace Lykke.Frontend.WampHost.Core.Mt
{
    public interface IMtRabbitMqHandler
    {
        void ProcessTrades(TradeContract trade);
        void ProcessAccountChanged(AccountChangedMessage accountChangedMessage);
        void ProcessOrderChanged(OrderContract order);
        void ProcessAccountStopout(AccountStopoutBackendContract stopout);
        void ProcessUserUpdates(UserUpdateEntityBackendContract userUpdate);
    }
}
