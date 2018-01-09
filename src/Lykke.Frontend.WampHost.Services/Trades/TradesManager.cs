using Lykke.Frontend.WampHost.Core.Services.Security;
using Lykke.Frontend.WampHost.Core.Services.Trades;
using Lykke.Job.TradesConverter.Contract;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Trades
{
    public class TradesManager : ITradesManager
    {
        private readonly IWampHostedRealm _realm;
        private readonly IClientResolver _clientResolver;

        public TradesManager(
            IWampHostedRealm realm,
            IClientResolver clientResolver)
        {
            _realm = realm;
            _clientResolver = clientResolver;
        }

        public void ProcessTrade(TradeLogItem message)
        {
            string notificationId = _clientResolver.GetNotificationId(message.UserId);

            if (string.IsNullOrEmpty(notificationId)) 
                return;
            
            var topic = $"trades.{notificationId}";
            var subject = _realm.Services.GetSubject<TradeLogItem>(topic);
            subject.OnNext(message);
        }
    }
}
