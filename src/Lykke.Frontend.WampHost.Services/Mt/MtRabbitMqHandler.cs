using System;
using System.Collections.Concurrent;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Frontend.WampHost.Core.Mt;
using MarginTrading.Contract.BackendContracts;
using MarginTrading.Contract.ClientContracts;
using MarginTrading.Contract.Mappers;
using MarginTrading.Contract.RabbitMqMessageModels;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Mt
{
    public class MtRabbitMqHandler: IMtRabbitMqHandler
    {
        private readonly ILog _log;
        private readonly ISubject<TradeClientContract> _tradesSubject;
        private readonly ISubject<NotifyResponse> _userUpdatesSubject;

        public MtRabbitMqHandler(IWampHostedRealm realm, ILog log)
        {
            _log = log;
            _userUpdatesSubject = realm.Services.GetSubject<NotifyResponse>("mt.user.updates");
            _tradesSubject = realm.Services.GetSubject<TradeClientContract>("mt.trades");
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

            _userUpdatesSubject.OnNext(new NotifyResponse { Account = accountChangedMessage.Account.ToClientContract() });
        }

        public void ProcessOrderChanged(OrderContract order)
        {
            _userUpdatesSubject.OnNext(new NotifyResponse { Order = order.ToClientContract() });
        }

        public void ProcessAccountStopout(AccountStopoutBackendContract stopout)
        {
            _userUpdatesSubject.OnNext(new NotifyResponse { AccountStopout = stopout.ToClientContract() });
        }

        public void ProcessUserUpdates(UserUpdateEntityBackendContract userUpdate)
        {
            foreach (var clientId in userUpdate.ClientIds)
            {
                try
                {
                    _userUpdatesSubject.OnNext(new NotifyResponse {UserUpdate = userUpdate.ToClientContract()});
                }
                catch (Exception ex)
                {
                    _log.WriteErrorAsync(nameof(MtRabbitMqHandler), nameof(ProcessUserUpdates), clientId, ex);
                    throw;
                }
            }
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
