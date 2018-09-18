﻿using System;
using System.Collections.Generic;
using Autofac;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Cqrs.Configuration;
using Lykke.Messaging;
using Lykke.Messaging.RabbitMq;
using Lykke.Cqrs;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.Assets.IncomeMessages;
using Lykke.Frontend.WampHost.Services.Projections;
using Lykke.Job.HistoryExportBuilder.Contract;
using Lykke.Job.HistoryExportBuilder.Contract.Events;
using Lykke.Messaging.Serialization;
using Lykke.Service.Assets.Contract.Events;
using Lykke.Service.Session.Contracts;
using Lykke.SettingsReader;

namespace Lykke.Frontend.WampHost.Modules
{
    internal class CqrsModule : Module
    {
        private readonly IReloadingManager<WampHostSettings> _settings;
        private readonly ILog _log;
        private readonly string _env;

        public CqrsModule(IReloadingManager<WampHostSettings> settings, ILog log, [CanBeNull] string env)
        {
            _settings = settings;
            _log = log;
            _env = env ?? "DefaultEnv";
        }

        protected override void Load(ContainerBuilder builder)
        {
            Messaging.Serialization.MessagePackSerializerFactory.Defaults.FormatterResolver = MessagePack.Resolvers.ContractlessStandardResolver.Instance;
            var rabbitMqSettings = new RabbitMQ.Client.ConnectionFactory { Uri = _settings.CurrentValue.SagasRabbitMqSettings.ConnectionString };

            builder.Register(context => new AutofacDependencyResolver(context)).As<IDependencyResolver>().SingleInstance();

            var messagingEngine = new MessagingEngine(_log,
                new TransportResolver(new Dictionary<string, TransportInfo>
                {
                    {"RabbitMq", new TransportInfo(rabbitMqSettings.Endpoint.ToString(), rabbitMqSettings.UserName, rabbitMqSettings.Password, "None", "RabbitMq")}
                }),
                new RabbitMqTransportFactory());

            builder.RegisterType<AssetsProjection>();
            builder.RegisterType<HistoryExportProjection>();

            builder.Register(ctx =>
            {
                const string defaultRoute = "self";

                return new CqrsEngine(_log,
                    ctx.Resolve<IDependencyResolver>(),
                    messagingEngine,
                    new DefaultEndpointProvider(),
                    true,
                    Register.DefaultEndpointResolver(new RabbitMqConventionEndpointResolver(
                        "RabbitMq",
                        SerializationFormat.MessagePack,
                        environment: "lykke",
                        exclusiveQueuePostfix: _env)),

                    Register.BoundedContext("wamp")
                        .ListeningEvents(
                                typeof(AssetCreatedEvent),
                                typeof(AssetUpdatedEvent),
                                typeof(AssetPairCreatedEvent),
                                typeof(AssetPairUpdatedEvent))
                            .From(BoundedContexts.Assets).On(defaultRoute)
                        .WithProjection(typeof(AssetsProjection), BoundedContexts.Assets)
                        .ListeningEvents(
                                typeof(ClientHistoryExportedEvent))
                            .From(HistoryExportBuilderBoundedContext.Name).On(defaultRoute)
                        .WithProjection(typeof(HistoryExportProjection), HistoryExportBuilderBoundedContext.Name)
                );
            })
            .As<ICqrsEngine>()
            .SingleInstance()
            .AutoActivate();
        }
    }

    internal class AutofacDependencyResolver : IDependencyResolver
    {
        private readonly IComponentContext _context;

        public AutofacDependencyResolver(IComponentContext kernel)
        {
            _context = kernel ?? throw new ArgumentNullException(nameof(kernel));
        }

        public object GetService(Type type)
        {
            return _context.Resolve(type);
        }

        public bool HasService(Type type)
        {
            return _context.IsRegistered(type);
        }
    }
}
