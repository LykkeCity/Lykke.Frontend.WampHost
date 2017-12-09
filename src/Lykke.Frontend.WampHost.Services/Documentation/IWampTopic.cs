using JetBrains.Annotations;
using Lykke.Frontend.WampHost.Services.Candles;
using Lykke.Frontend.WampHost.Services.Quotes.Contract;

namespace Lykke.Frontend.WampHost.Services.Documentation
{
    public interface IWampTopics
    {
        [DocMe(Name = "candle.{spot|mt}.{instrument}.{bid|ask|mid}.{sec|minute|min5|min15|min30|hour|hour4|hour6|hour12|day|week|month}", Description = "provides candles. realm = 'prices', all parameters in the lower case.")]
        CandleClientMessage CandlesUpdate();

        [UsedImplicitly]
        [DocMe(Name = "quote.{spot|mt}.{instrument}.{bid|ask}", Description = "provides quotes. realm = 'prices', all parameters in the lower case.")]
        QuoteClientMessage Quotes();
    }
}
