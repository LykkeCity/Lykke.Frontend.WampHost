using Lykke.SettingsReader.Attributes;

namespace Lykke.Frontend.WampHost.Settings
{
    public class ClientAccountSettings
    {
        [HttpCheck("/api/isalive")]
        public string ServiceUrl { get; set; }
    }
}
