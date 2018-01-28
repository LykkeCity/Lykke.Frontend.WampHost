using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Services.Candles.Contract;
using Lykke.Frontend.WampHost.Services.Quotes.Contract;
using MarginTrading.Contract.ClientContracts;

namespace Lykke.Frontend.WampHost.Services.Documentation
{
    public interface IWampTopics
    {
        [DocMe(Name = "candle.{spot|mt}.{instrument}.{bid|ask|mid}.{sec|minute|min5|min15|min30|hour|hour4|hour6|hour12|day|week|month}", Description = "provides candles. realm = 'prices', all parameters in the lower case.")]
        [UsedImplicitly]
        CandleClientMessage CandlesUpdate();

        [UsedImplicitly]
        [DocMe(Name = "quote.{spot|mt}.{instrument}.{bid|ask}", Description = "provides quotes. realm = 'prices', all parameters in the lower case.")]
        QuoteClientMessage Quotes();
        
        [UsedImplicitly]
        [DocMe(Name = "trades.mt", Description = "provides margin trades of all users. realm = 'prices'.")]
        TradeClientContract MtTrades();
        
        [UsedImplicitly]
        [DocMe(Name = "user-updates.mt", Description = "provides margin trading users and accounts updates. realm = 'prices'.")]
        NotifyResponse MtUserUpdates();
    }
}
