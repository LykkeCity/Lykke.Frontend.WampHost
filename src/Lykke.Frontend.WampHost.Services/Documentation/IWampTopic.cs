using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Services.Balances.Contracts;
using Lykke.Frontend.WampHost.Core.Services.Orderbook;
using Lykke.Frontend.WampHost.Services.Candles.Contract;
using Lykke.Frontend.WampHost.Services.Quotes.Contract;
using Lykke.Job.TradesConverter.Contract;

namespace Lykke.Frontend.WampHost.Services.Documentation
{
    public interface IWampTopics
    {
        [DocMe(Name = "candle.{spot|mt}.{instrument}.{bid|ask|mid}.{sec|minute|min5|min15|min30|hour|hour4|hour6|hour12|day|week|month}", Description = "provides candles. realm = 'prices', all parameters in the lower case.")]
        CandleClientMessage CandlesUpdate();

        [UsedImplicitly]
        [DocMe(Name = "quote.{spot|mt}.{instrument}.{bid|ask}", Description = "provides quotes. realm = 'prices', all parameters in the lower case.")]
        QuoteClientMessage Quotes();

        [UsedImplicitly]
        [DocMe(Name = "balances", Description = "user balances updates. realm = 'user', all parameters in the lower case.")]
        BalanceUpdateMessage BalancesUpdate();
        
        [UsedImplicitly]
        [DocMe(Name = "spot.orderbook.{instrument}.{buy|sell}", Description = "provides orderbooks. realm = 'prices', all parameters in the lower case.")]
        OrderbookModel Orderbooks();
        
        [UsedImplicitly]
        [DocMe(Name = "trades", Description = "provides trades for a specific user. realm = 'prices', all parameters in the lower case.")]
        TradeLogItem Trades();
    }
}
