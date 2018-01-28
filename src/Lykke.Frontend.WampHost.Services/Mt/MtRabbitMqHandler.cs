using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Frontend.WampHost.Core.Mt;
using Lykke.Frontend.WampHost.Core.Services.Security;
using MarginTrading.Contract.BackendContracts;
using MarginTrading.Contract.ClientContracts;
using MarginTrading.Contract.Mappers;
using MarginTrading.Contract.RabbitMqMessageModels;
using WampSharp.V2;
using WampSharp.V2.Core.Contracts;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Mt
{
    public class MtRabbitMqHandler: IMtRabbitMqHandler
    {
        private readonly ILog _log;
        private readonly IClientResolver _clientResolver;
        private readonly ISubject<TradeClientContract> _tradesSubject;
        private readonly IWampSubject _userUpdatesSubject;

        public MtRabbitMqHandler(IWampHostedRealm realm, ILog log, IClientResolver clientResolver)
        {
            _log = log;
            _clientResolver = clientResolver;
            _userUpdatesSubject = realm.Services.GetSubject("user-updates.mt");
            _tradesSubject = realm.Services.GetSubject<TradeClientContract>("trades.mt");
        }
        
        public void ProcessTrades(TradeContract trade)
        {
            _tradesSubject.OnNext(new TradeClientContract
            {
                Id = trade.Id,
                AssetPairId = trade.AssetPairId,
                Date = trade.Date,
                OrderId = trade.OrderId,
                Price = trade.Price,
                Type = ConvertEnum<TradeClientType>(trade.Type),
                Volume = trade.Volume
            });
        }

        public void ProcessAccountChanged(AccountChangedMessage accountChangedMessage)
        {
            if (accountChangedMessage.EventType != AccountEventTypeEnum.Updated)
            {
                return;
            }

            SendUserUpdate(new NotifyResponse { Account = accountChangedMessage.Account.ToClientContract() }, accountChangedMessage.Account.ClientId);
        }

        public void ProcessOrderChanged(OrderContract order)
        {
            SendUserUpdate(new NotifyResponse { Order = order.ToClientContract() }, order.ClientId);
        }

        public void ProcessAccountStopout(AccountStopoutBackendContract stopout)
        {
            SendUserUpdate(new NotifyResponse { AccountStopout = stopout.ToClientContract() }, stopout.ClientId);
        }

        public void ProcessUserUpdates(UserUpdateEntityBackendContract userUpdate)
        {
            var exceptions = new List<Exception>();
            foreach (var clientId in userUpdate.ClientIds)
            {
                try
                {
                    SendUserUpdate(new NotifyResponse {UserUpdate = userUpdate.ToClientContract()}, clientId);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }

        private void SendUserUpdate(NotifyResponse notifyResponse, string clientId)
        {
            var notificationId = _clientResolver.GetNotificationId(clientId);
            _userUpdatesSubject.OnNext(new WampEvent
            {
                Options = new PublishOptions {Eligible = new[] {long.Parse(notificationId)}},
                Arguments = new object[] {notifyResponse}
            });
        }

        private static TEnum ConvertEnum<TEnum>(Enum dto)
            where TEnum : struct, IConvertible
        {
            if (!Enum.TryParse(dto.ToString(), out TEnum result))
            {
                throw new NotSupportedException($"Value {dto} is not supported by mapper");
            }

            return result;
        }
    }
}
