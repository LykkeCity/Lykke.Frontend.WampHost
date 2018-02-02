using Lykke.SettingsReader.Attributes;

namespace Lykke.Frontend.WampHost.Settings
{
    public class SessionServiceSettings
    {
        [HttpCheck("/api/isalive")]
        public string SessionServiceUrl { get; set; }
    }
}
