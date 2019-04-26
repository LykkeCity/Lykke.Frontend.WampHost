using System.Threading.Tasks;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Services.EasyBuy.Contract;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.EasyBuy.Contract;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.EasyBuy
{
    public class EasyBuyPricesSubscriber : ISubscriber
    {
        private readonly IWampHostedRealm _realm;
        private readonly IRabbitMqSubscribeHelper _rabbitMqSubscribeHelper;
        private readonly string _connectionString;

        public EasyBuyPricesSubscriber(
            IWampHostedRealm realm, 
            IRabbitMqSubscribeHelper rabbitMqSubscribeHelper,
            string connectionString)
        {
            _realm = realm;
            _rabbitMqSubscribeHelper = rabbitMqSubscribeHelper;
            _connectionString = connectionString;
        }

        public void Start()
        {
            _rabbitMqSubscribeHelper.Subscribe(
                _connectionString,
                 MarketType.Spot,
                "lykke.easyBuy.prices",
                new JsonMessageDeserializer<Price>(),
                ProcessPriceAsync);
        }

        private Task ProcessPriceAsync(Price updatedPrice)
        {
            var subject = _realm.Services.GetSubject<PriceUpdateMessage>($"easybuy.price.{updatedPrice.AssetPair}");
            
            subject.OnNext(new PriceUpdateMessage
            {
                Id = updatedPrice.Id,
                AssetPair = updatedPrice.AssetPair,
                Value = updatedPrice.Value,
                BaseVolume = updatedPrice.BaseVolume,
                QuoteVolume = updatedPrice.QuoteVolume,
                ValidFrom = updatedPrice.ValidFrom,
                ValidTo = updatedPrice.ValidTo
            });
            
            return Task.CompletedTask;
        }
    }
}
