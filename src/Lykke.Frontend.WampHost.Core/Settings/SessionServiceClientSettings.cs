using Lykke.SettingsReader.Attributes;

namespace Lykke.Frontend.WampHost.Core.Settings
{
    public class SessionServiceClientSettings
    {
        [HttpCheck("/api/isalive")]
        public string SessionServiceUrl { get; set; }
    }
}
