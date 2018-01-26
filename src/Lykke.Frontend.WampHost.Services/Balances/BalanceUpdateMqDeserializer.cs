using System.Text;
using Common;
using Lykke.Frontend.WampHost.Services.Balances.IncomeMessages;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Frontend.WampHost.Services.Balances
{
    public class BalanceUpdateMqDeserializer : IMessageDeserializer<BalanceUpdateEventModel>
    {
        public BalanceUpdateEventModel Deserialize(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
            return json.DeserializeJson<BalanceUpdateEventModel>();
        }
    }
}
