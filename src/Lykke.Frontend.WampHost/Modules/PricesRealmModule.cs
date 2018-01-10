﻿using Autofac;
using Autofac.Core;
using Lykke.Frontend.WampHost.Core.Domain;
using Lykke.Frontend.WampHost.Core.Services;
using Lykke.Frontend.WampHost.Core.Services.Candles;
using Lykke.Frontend.WampHost.Core.Services.Quotes;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.Candles;
using Lykke.Frontend.WampHost.Services.Quotes;
using Lykke.Frontend.WampHost.Services.Quotes.Mt;
using Lykke.Frontend.WampHost.Services.Quotes.Spot;
using WampSharp.V2;
using WampSharp.V2.Realm;

namespace Lykke.Frontend.WampHost.Modules
{
    public class PricesRealmModule : Module
    {
        private readonly WampHostSettings _settings;

        public PricesRealmModule(WampHostSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            const string realm = "prices";

            builder.Register(x => x.Resolve<IWampHost>().RealmContainer.GetRealmByName(realm));

            RegiserCandles(builder, realm);
            RegiserQuotes(builder, realm);
        }

        private void RegiserCandles(ContainerBuilder builder, string realm)
        {
            builder.RegisterType<CandlesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(MarketType.Spot))
                .WithParameter(TypedParameter.From(_settings.RabbitMqSettings.ConnectionString))
                .PreserveExistingDefaults();

            builder.RegisterType<CandlesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(MarketType.Mt))
                .WithParameter(TypedParameter.From(_settings.MtRabbitMqSettings.ConnectionString))
                .PreserveExistingDefaults();

            builder.RegisterType<CandlesManager>()
                .WithParameter(
                    new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(IWampHostedRealm),
                        (pi, ctx) => ctx.Resolve<IWampHost>().RealmContainer.GetRealmByName(realm)))
                .As<ICandlesManager>()
                .SingleInstance();
        }

        private void RegiserQuotes(ContainerBuilder builder, string realm)
        {
            builder.RegisterType<SpotQuotesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.SpotQuotesRabbitMqSettings.ConnectionString))
                .PreserveExistingDefaults();

            builder.RegisterType<MtQuotesSubscriber>()
                .As<ISubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.MtQuotesRabbitMqSettings.ConnectionString))
                .PreserveExistingDefaults();

            builder.RegisterType<QuotesManager>()
                .WithParameter(
                    new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(IWampHostedRealm),
                        (pi, ctx) => ctx.Resolve<IWampHost>().RealmContainer.GetRealmByName(realm)))
                .As<IQuotesManager>()
                .SingleInstance();
        }
    }
}