using Lykke.SettingsReader.Attributes;

namespace Lykke.Frontend.WampHost.Core.Settings
{
    public class AssetServiceClientSettings
    {
        [HttpCheck("/api/isalive")]
        public string ServiceUrl { get; set; }
    }
}
