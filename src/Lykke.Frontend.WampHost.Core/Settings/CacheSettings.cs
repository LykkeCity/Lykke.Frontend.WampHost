using System;

namespace Lykke.Frontend.WampHost.Core.Settings
{
    public class CacheSettings
    {
        public string Instance { set; get; }
        public string TradeAnonIdKeyPattern { set; get; }
        public TimeSpan MarketDataCacheInterval { get; set; }
    }

    public static class CacheSettingsExt
    {
        public static string GetKeyForTradeAnonId(this CacheSettings settings, string id)
        {
            return string.Format(settings.TradeAnonIdKeyPattern, id);
        }
    }
}
