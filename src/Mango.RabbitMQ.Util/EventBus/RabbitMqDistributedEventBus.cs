using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mango.RabbitMQ.Util.EventBus
{
    public class RabbitMqDistributedEventBus
    {
        protected RabbitMqEventBusOptions RabbitMqEventBusOptions { get; }
        //protected DistributedEventBusOptions DistributedEventBusOptions { get; }
        protected IConnectionPool ConnectionPool { get; }
        protected IRabbitMqSerializer Serializer { get; }

        protected ConcurrentDictionary<Type, List<Object>> HandlerFactories { get; }
        protected ConcurrentDictionary<string, Type> EventTypes { get; }
        protected IRabbitMqMessageConsumerFactory MessageConsumerFactory { get; }
        protected IRabbitMqMessageConsumer Consumer { get; private set; }

        public RabbitMqDistributedEventBus(
           RabbitMqEventBusOptions options,
           IConnectionPool connectionPool,
           IRabbitMqSerializer serializer,
           IRabbitMqMessageConsumerFactory messageConsumerFactory)
        {
            ConnectionPool = connectionPool;
            Serializer = serializer;
            MessageConsumerFactory = messageConsumerFactory;
            RabbitMqEventBusOptions = options;

            HandlerFactories = new ConcurrentDictionary<Type, List<Object>>();
            EventTypes = new ConcurrentDictionary<string, Type>();
        }

        public void Initialize()
        {
            Consumer = MessageConsumerFactory.Create(
                new ExchangeDeclareConfiguration(
                    RabbitMqEventBusOptions.ExchangeName,
                    type: "direct",
                    durable: true
                ),
                new QueueDeclareConfiguration(
                    RabbitMqEventBusOptions.ClientName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                ),
                RabbitMqEventBusOptions.ConnectionName
            );

            Consumer.OnMessageReceived(ProcessEventAsync);

            //SubscribeHandlers(AbpDistributedEventBusOptions.Handlers);
        }

        private async Task ProcessEventAsync(IModel channel, BasicDeliverEventArgs ea)
        {
            var eventName = ea.RoutingKey;
            //var eventType = EventTypes.GetOrDefault(eventName);
            _ = EventTypes.TryGetValue(eventName, out var eventType) ? eventType : default;
            if (eventType == null)
            {
                return;
            }

            var eventData = Serializer.Deserialize(ea.Body.ToArray(), eventType);

            //await TriggerHandlersAsync(eventType, eventData);
        }


        public Task PublishAsync(string routingKey, object eventData)
        {
            var eventName = routingKey;
            var body = Serializer.Serialize(eventData);

            using (var channel = ConnectionPool.Get(RabbitMqEventBusOptions.ConnectionName).CreateModel())
            {
                channel.ExchangeDeclare(
                    RabbitMqEventBusOptions.ExchangeName,
                    "direct",
                    durable: true
                );

                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = RabbitMqConsts.DeliveryModes.Persistent;

                channel.BasicPublish(
                   exchange: RabbitMqEventBusOptions.ExchangeName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body
                );
            }

            return Task.CompletedTask;
        }

        public void Subscribe(Type eventType, Object factory)
        {
            var handlerFactories = GetOrCreateHandlerFactories(eventType);

            //if (factory.IsInFactories(handlerFactories))
            //{
            //    return NullDisposable.Instance;
            //}

            handlerFactories.Add(factory);

            if (handlerFactories.Count == 1) //TODO: Multi-threading!
            {
                var eventName = eventType.Name ?? eventType.FullName;
                Consumer.BindAsync(eventName);

            }

            //return new EventHandlerFactoryUnregistrar(this, eventType, factory);
        }

        ///// <inheritdoc/>
        //public override void Unsubscribe(Type eventType, IEventHandler handler)
        //{
        //    GetOrCreateHandlerFactories(eventType)
        //        .Locking(factories =>
        //        {
        //            factories.RemoveAll(
        //                factory =>
        //                    factory is SingleInstanceHandlerFactory &&
        //                    (factory as SingleInstanceHandlerFactory).HandlerInstance == handler
        //            );
        //        });
        //}

        private List<Object> GetOrCreateHandlerFactories(Type eventType)
        {
            return HandlerFactories.GetOrAdd(
                eventType,
                type =>
                {
                    //var eventName = EventNameAttribute.GetNameOrDefault(type);
                    var eventName = type.Name ?? type.FullName;
                    EventTypes[eventName] = type;
                    return new List<object>();
                }
            );
        }
    }
}
