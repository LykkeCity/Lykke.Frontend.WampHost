using Lykke.Frontend.WampHost.Services.Documentation;

namespace Lykke.Frontend.WampHost.Models
{
    public class MethodInfoModel
    {
        public MethodDocInfo[] Rpc { get; set; }
        public MethodDocInfo[] Topic { get; set; }
    }
}