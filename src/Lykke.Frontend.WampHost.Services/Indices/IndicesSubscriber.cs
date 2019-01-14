using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Job.TradesConverter.Contract;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.IndicesFacade.Contract;
using MongoDB.Bson;
using WampSharp.V2;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Services.Indices
{
    public class IndicesSubscriber : ISubscriber
    {
        private readonly IRabbitMqSubscribeHelper _rabbitMqSubscribeHelper;
        private readonly string _connectionString;
        private readonly IWampHostedRealm _realm;

        public IndicesSubscriber(
            IRabbitMqSubscribeHelper rabbitMqSubscribeHelper,
            string connectionString,
            IWampHostedRealm realm)
        {
            _rabbitMqSubscribeHelper = rabbitMqSubscribeHelper;
            _connectionString = connectionString;
            _realm = realm;
        }
        
        public void Start()
        {
            _rabbitMqSubscribeHelper.Subscribe(
                connectionString: _connectionString,
                market: MarketType.Spot,
                source: "indicesFacade.indicesUpdates",
                deserializer: new JsonMessageDeserializer<Index>(),
                handler: ProcessUpdateAsync);
        }

        private Task ProcessUpdateAsync(Index arg)
        {
            var topic = $"indices.{arg.AssetId}";
            
            var subject = _realm.Services.GetSubject<Index>(topic);

            subject.OnNext(arg);

            return Task.CompletedTask;
        }
    }
}
