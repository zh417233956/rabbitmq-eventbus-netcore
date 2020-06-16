using Mango.RabbitMQ.Core;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Publish.App.Module
{
    public class RabbitMqEventBus
    {
        protected RabbitMqEventBusOptions RabbitMqEventBusOptions { get; }
        protected IConnectionPool ConnectionPool { get; }
        protected IRabbitMqSerializer Serializer { get; }
        protected IRabbitMqMessageConsumerFactory MessageConsumerFactory { get; }
        protected IRabbitMqMessageConsumer Consumer { get; private set; }

        public RabbitMqEventBus(
            IOptions<RabbitMqEventBusOptions> options,
            IConnectionPool connectionPool,
            IRabbitMqSerializer serializer,
            IRabbitMqMessageConsumerFactory messageConsumerFactory)
        {
            ConnectionPool = connectionPool;
            Serializer = serializer;
            MessageConsumerFactory = messageConsumerFactory;
            RabbitMqEventBusOptions = options.Value;
            Initialize();
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
        }

        public Task PublishAsync(string routingKey, Object eventData)
        {
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
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body
                );
            }

            return Task.CompletedTask;
        }
        public Task BindAsync(string routingKey)
        {
            return Consumer.BindAsync(routingKey);
        }
        public Task UnbindAsync(string routingKey)
        {
            return Consumer.UnbindAsync(routingKey);
        }
    }
    public class RabbitMqEventBusOptions
    {
        public string ConnectionName { get; set; }

        public string ClientName { get; set; }

        public string ExchangeName { get; set; }
    }
}
