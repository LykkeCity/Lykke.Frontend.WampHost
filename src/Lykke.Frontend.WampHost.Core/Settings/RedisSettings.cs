namespace Lykke.Frontend.WampHost.Core.Settings
{
    public class RedisSettings
    {
        public string Configuration { set; get; }
        public string Instance { set; get; }
        public string TradeAnonIdKeyPattern { set; get; }
    }

    public static class RedisSettingsExt
    {
        public static string GetKeyForTradeAnonId(this RedisSettings settings, string id)
        {
            return string.Format(settings.TradeAnonIdKeyPattern, id);
        }
    }
}
