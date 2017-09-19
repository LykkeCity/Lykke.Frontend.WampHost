using Lykke.Frontend.WampHost.Services.Candles;

namespace Lykke.Frontend.WampHost.Services.Documentation
{
    public interface IWampTopics
    {
        [DocMe(Name = "candle.{spot|mt}.{instrument}.{bid|ask|mid}.{sec|minute|min5|min15|min30|hour|hour4|hour6|hour12|day|week|month}", Description = "sends candles. realm = 'prices', all parameters in the lower case.")]
        CandleMessage CandlesUpdate();        
    }
}