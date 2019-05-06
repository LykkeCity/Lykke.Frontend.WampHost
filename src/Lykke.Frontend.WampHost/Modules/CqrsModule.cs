using System;
using System.Collections.Generic;
using Autofac;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Exchange.Api.MarketData.Contract;
using Lykke.Frontend.WampHost.Contracts;
using Lykke.Frontend.WampHost.Contracts.Commands;
using Lykke.Frontend.WampHost.Core.Settings;
using Lykke.Frontend.WampHost.Services.Assets.IncomeMessages;
using Lykke.Frontend.WampHost.Services.CommandHandlers;
using Lykke.Frontend.WampHost.Services.Projections;
using Lykke.Job.HistoryExportBuilder.Contract;
using Lykke.Job.HistoryExportBuilder.Contract.Events;
using Lykke.Messaging;
using Lykke.Messaging.Contract;
using Lykke.Messaging.RabbitMq;
using Lykke.Messaging.Serialization;
using Lykke.Service.Assets.Contract.Events;
using Lykke.Service.Operations.Contracts;
using Lykke.Service.Operations.Contracts.Events;
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
            var rabbitMqSettings = new RabbitMQ.Client.ConnectionFactory
            {
                Uri = _settings.CurrentValue.SagasRabbitMqSettings.ConnectionString
            };

            builder.Register(context => new AutofacDependencyResolver(context)).As<IDependencyResolver>().SingleInstance();

            builder.Register(c=> 
             new MessagingEngine(_log,
                new TransportResolver(new Dictionary<string, TransportInfo>
                {
                    {"RabbitMq", new TransportInfo(rabbitMqSettings.Endpoint.ToString(), rabbitMqSettings.UserName, rabbitMqSettings.Password, "None", "RabbitMq")}
                }),
                new RabbitMqTransportFactory()))
                .As<IMessagingEngine>();
            
            builder.RegisterType<ConfirmationCommandHandler>();

            builder.RegisterType<AssetsProjection>();
            builder.RegisterType<HistoryExportProjection>();
            builder.RegisterType<OperationsProjection>();
            builder.RegisterType<MarketDataProjection>();
            
            var protobufEndpointResolver = new RabbitMqConventionEndpointResolver(
                "RabbitMq",
                SerializationFormat.ProtoBuf,
                environment: "lykke",
                exclusiveQueuePostfix: _env);

            builder.Register(ctx =>
            {
                const string defaultRoute = "self";

                var engine = new CqrsEngine(
                    _log,
                    ctx.Resolve<IDependencyResolver>(),
                    ctx.Resolve<IMessagingEngine>(),
                    new DefaultEndpointProvider(),
                    true,
                    Register.DefaultEndpointResolver(new RabbitMqConventionEndpointResolver(
                        "RabbitMq",
                        SerializationFormat.MessagePack,
                        environment: "lykke",
                        exclusiveQueuePostfix: _env)),

                    Register.BoundedContext(WampHostBoundedContext.Name)
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
                        .ListeningEvents(typeof(OperationFailedEvent), typeof(OperationConfirmedEvent), typeof(OperationCompletedEvent), typeof(OperationCorruptedEvent))
                            .From(OperationsBoundedContext.Name).On(defaultRoute)
                        .WithProjection(typeof(OperationsProjection), OperationsBoundedContext.Name)
                        .ListeningEvents(typeof(MarketDataChangedEvent))
                            .From(MarketDataBoundedContext.Name).On(defaultRoute)
                        .WithProjection(typeof(MarketDataProjection), MarketDataBoundedContext.Name)
                        .ListeningCommands(typeof(RequestConfirmationCommand)).On("commands")
                        .WithEndpointResolver(protobufEndpointResolver)
                        .WithCommandsHandler<ConfirmationCommandHandler>()
                );
                engine.StartPublishers();
                return engine;
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
