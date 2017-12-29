namespace Lykke.Frontend.WampHost.Core.Services.Orderbook
{
    public interface IOrderbookManager
    {
        void ProcessOrderbook(OrderbookMessage orderbookMessage);
    }
}
